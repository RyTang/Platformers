using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Run")]
public class NormalRunState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl, attackControl, sprintControl;

    private Rigidbody2D rb2d;

    private bool canJump = false;
    private Coroutine coyoteTimer;


    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        rb2d = parent.GetRigidbody2D();
        canJump = false;
        Runner.GetAnimator().SetBool(PlayerAnimation.isRunningBool, true);
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        dashControl = Runner.GetDashControls();
        attackControl = Runner.GetAttackControls();
        sprintControl = Runner.GetSprintControls();
    }

    public override void CheckStateTransition()
    {
        if (dashControl > 0){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(NormalDashState)));
        }
        else if (attackControl > 0) {
            CurrentSuperState.SetSubState(Runner.GetState(typeof(GroundSubAttackOne)));
        }
        else if (horizontalControl == 0){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(NormalIdleState)));
        }
        else if (verticalControl > 0 && canJump) {
            CurrentSuperState.SetSubState(Runner.GetState(typeof(NormalJumpState)));
        }
        else if ((verticalControl < 0 && !Runner.GetGroundCheck().Check()) || (Runner.GetRigidbody2D().velocity.y < 0 && !canJump)){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(NormalFallState)));
        }
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check()){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(NormalWallClingState)));
        }
    }
    
    public override IEnumerator ExitState(){
        canJump = false;
        Runner.GetAnimator().SetBool(PlayerAnimation.isRunningBool, false);
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
            rb2d.velocity = horizontalControl > 0 ? new Vector2(Runner.GetPlayerData().moveSpeed, rb2d.velocity.y) : new Vector2(-Runner.GetPlayerData().moveSpeed, rb2d.velocity.y);
        }
        
        CheckGround();
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