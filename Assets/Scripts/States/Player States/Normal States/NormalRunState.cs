using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Run")]
public class NormalRunState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl, attackControl, mobilityControl;
    private float sprintControl;
    private Rigidbody2D rb2d;


    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        rb2d = parent.GetRigidbody2D();
        Runner.GetAnimator().SetBool(PlayerAnimation.isRunningBool, true);
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        dashControl = Runner.GetDashControls();
        attackControl = Runner.GetAttackControls();
        mobilityControl = Runner.GetMobilityControl();
        sprintControl = Runner.GetSprintControls();
    }

    public override void CheckStateTransition()
    {
        if (dashControl > 0){
            CurrentSuperState.SetSubState(typeof(NormalDashState));
        }
        else if (attackControl > 0) {
            CurrentSuperState.SetSubState(typeof(GroundSubAttackOne));
        }
        else if (horizontalControl == 0){
            CurrentSuperState.SetSubState(typeof(NormalIdleState));
        }
        else if (verticalControl > 0) {
            CurrentSuperState.SetSubState(typeof(NormalJumpState));
        }
        else if (!Runner.GetGroundCheck().Check() && (verticalControl < 0 || Runner.GetRigidbody2D().velocity.y < 0)){
            CurrentSuperState.SetSubState(typeof(NormalFallCoyoteState));
        }
        else if (mobilityControl > 0) {
            CurrentSuperState.SetSubState(typeof(NormalSlideState));
        }
        else if (horizontalControl != 0){
            if (Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check()) {
                CurrentSuperState.SetSubState(typeof(NormalWallClingState));
            }
            else if (sprintControl > 0) {
                CurrentSuperState.SetSubState(typeof(NormalSprintState));
            }
        }
    }
    
    public override IEnumerator ExitState(){
        Runner.GetAnimator().SetBool(PlayerAnimation.isRunningBool, false);
        yield break;
    }

    public override void UpdateState()
    {
        if (horizontalControl != 0){
            rb2d.velocity = horizontalControl > 0 ? new Vector2(Runner.GetPlayerData().moveSpeed, rb2d.velocity.y) : new Vector2(-Runner.GetPlayerData().moveSpeed, rb2d.velocity.y);
        }
        
    }
}