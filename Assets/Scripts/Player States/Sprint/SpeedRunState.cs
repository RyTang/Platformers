using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Speed State/Run State")]
public class SpeedRunState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl, attackControl, sprintControl;

    private Rigidbody2D rb2d;

    private bool isSprinting = true;

    private bool canJump = false;
    private Coroutine coyoteTimer;
    private Coroutine energyConsumption;
    private Coroutine energyRecovery;

    private float currentSprintDirection = 0;
    private float currentSpeed = 0.0f;
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

        horizontalControl = Runner.GetHorizontalControls();
        ResetSprint(horizontalControl);
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
            Runner.GetPlayerData().currentEnergy = Mathf.Min(Runner.GetPlayerData().currentEnergy, Runner.GetPlayerData().maxEnergyBar);
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
            Runner.SetMainState(typeof(SpeedJumpState), Runner.GetRigidbody2D().velocity.x);
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
            AccelerateTowards();
        }
        CheckGround();
    }

    private void ResetSprint(float direction){
        if (currentSprintDirection != direction){
            currentSpeed = 0;
            currentSprintDirection = direction;
        }
        else{
            currentSpeed = Mathf.Abs(rb2d.velocity.x);
        }
        
    }

    private void AccelerateTowards(){       
        if (currentSprintDirection != horizontalControl){
            ResetSprint(horizontalControl);
        }
        else if (currentSprintDirection == horizontalControl){
            currentSpeed += Runner.GetPlayerData().accelerationSpeed * Time.deltaTime;

            currentSpeed = Mathf.Clamp(currentSpeed, 0, Runner.GetPlayerData().maxSprintSpeed);
        }

        // FIXME: BUG WHEN CHANGIN DIRECTION
        // Change of direction
        // Decelerate if Change of Direction
        // else if (velocityDirection != direction){
        //     Debug.Log("Decelerating");
        //     decelerationTimer += Time.deltaTime;
        //     t = decelerationTimer / Runner.GetPlayerData().decelerationTime;

        //     currentSpeed = Mathf.SmoothStep(initialSpeed, Runner.GetPlayerData().maxSprintSpeed, t);
        // }
        // Else Accelerate
        
        rb2d.velocity = new Vector2(currentSpeed * currentSprintDirection, rb2d.velocity.y);
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