using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Sprint State")]
public class SprintState : BaseState<PlayerController>
{
     private float horizontalControl, verticalControl, dashControl, attackControl, sprintControl;

    private Rigidbody2D rb2d;

    private bool isSprinting = true;

    private bool canJump = false;
    private Coroutine coyoteTimer;
    private Coroutine energyConsumption;
    private Coroutine energyRecovery;

    private float currentDirection = 0;
    private float currentSpeed = 0.0f;
    private float initialSpeed = 0.0f;
    private float accelerationTimer = 0f;
    private float decelerationTimer = 0f;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        if (Runner.GetPlayerData().currentEnergy <= 0){
            return;
        }

        rb2d = parent.GetRigidbody2D();
        canJump = false;
        isSprinting = true;
        Runner.GetAnimator().SetBool(PlayerAnimation.isRunningBool, true);
        initialSpeed = rb2d.velocity.x;
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        dashControl = Runner.GetDashControls();
        attackControl = Runner.GetAttackControls();
        sprintControl = Runner.GetSprintControls();

        if (sprintControl > 0 && Runner.GetPlayerData().currentEnergy > 0){
            isSprinting = true;
            if (energyRecovery != null) {
                Runner.StopCoroutine(energyRecovery);
                energyRecovery = null;
            }
            energyConsumption ??= Runner.StartCoroutine(EnergyConsumption());            
        }
    }

    private IEnumerator EnergyConsumption(){
        while (isSprinting){
            Runner.GetPlayerData().currentEnergy -= Runner.GetPlayerData().sprintDepletionRate;

            if (Runner.GetPlayerData().currentEnergy <= 0){
                isSprinting = false;
            }

            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator EnergyRecovery(){
        while (Runner.GetPlayerData().currentEnergy < Runner.GetPlayerData().maxEnergyBar){
            Runner.GetPlayerData().currentEnergy += Runner.GetPlayerData().energyRecoveryRate;

            yield return new WaitForSeconds(1);
        }
    }

    public override void CheckStateTransition()
    {
        if (!isSprinting && horizontalControl != 0){
            Runner.SetMainState(typeof(WalkState));
        }
        else if (dashControl > 0){
            Runner.SetMainState(typeof(DashState));
        }
        else if (attackControl > 0) {
            Runner.SetMainState(typeof(GroundSubAttackOne));
        }
        else if (horizontalControl == 0){
            Runner.SetMainState(typeof(IdleState));
        }
        else if (verticalControl > 0 && canJump) {
            Runner.SetMainState(typeof(JumpState));
        }
        else if ((verticalControl < 0 && !Runner.GetGroundCheck().Check()) || (Runner.GetRigidbody2D().velocity.y < 0 && !canJump)){
            Runner.SetMainState(typeof(FallState));
        }
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check()){
            Runner.SetMainState(typeof(WallClingState));
        }
    }
    
    public override IEnumerator ExitState(){
        canJump = false;
        Runner.GetAnimator().SetBool(PlayerAnimation.isRunningBool, false);


        // FIXME: BUG when energy recovery may happen during consumption
    
        isSprinting = false;
        if (energyConsumption != null) {
            Runner.StopCoroutine(energyConsumption);
            energyConsumption = null;
        }
        energyRecovery ??= Runner.StartCoroutine(EnergyRecovery());
        yield break;
    }

    public override void FixedUpdateState()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
        
    }

    public override void UpdateState()
    {
        if (horizontalControl != 0){
            AccelerateTowards(horizontalControl);
        }
        CheckGround();
    }

    private void AccelerateTowards(float direction){
        float velocityDirection = Mathf.Sign(rb2d.velocity.x);
        float t = 0 ;
        
        // FIXME: BUG WHEN CHANGIN DIRECTION
        // Change of direction
        if (currentDirection != direction){
            Debug.Log("Changing Direction");
            // initialSpeed = Mathf.Abs(rb2d.velocity.x);
            initialSpeed = Mathf.Clamp(Mathf.Abs(rb2d.velocity.x), 0, Runner.GetPlayerData().maxSprintSpeed);
            currentDirection = direction;
            decelerationTimer = 0;
            accelerationTimer = 0;
        }
        // Decelerate if Change of Direction
        // else if (velocityDirection != direction){
        //     Debug.Log("Decelerating");
        //     decelerationTimer += Time.deltaTime;
        //     t = decelerationTimer / Runner.GetPlayerData().decelerationTime;

        //     currentSpeed = Mathf.SmoothStep(initialSpeed, Runner.GetPlayerData().maxSprintSpeed, t);
        // }
        // Else Accelerate
        else if (velocityDirection == direction){
            Debug.Log("Accelerating");
            accelerationTimer += Time.deltaTime;
            t = accelerationTimer / Runner.GetPlayerData().accelerationTime;
            currentSpeed = Mathf.SmoothStep(initialSpeed, Runner.GetPlayerData().maxSprintSpeed, t);
        }

        currentSpeed = Mathf.SmoothStep(initialSpeed, Runner.GetPlayerData().maxSprintSpeed, t);

        rb2d.velocity = new Vector2(currentSpeed * direction, rb2d.velocity.y);
    }

    public void CheckGround(){
        if (Runner.GetGroundCheck().Check()){
            canJump = true;
            if (coyoteTimer != null){
                Runner.StopCoroutine(coyoteTimer);
                coyoteTimer = null;
            }
        }   
        else {
            coyoteTimer = Runner.StartCoroutine(CoyoteTimer());
        }
    }

    public IEnumerator CoyoteTimer(){
        yield return new WaitForSeconds(Runner.GetPlayerData().coyoteTime);
        canJump = false;
    }


    public override void InitialiseSubState()
    {
    }
}