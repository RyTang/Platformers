using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Sprint State/Idle")]
public class SprintIdleState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl, attackControl, mobilityControl;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        Runner.GetAnimator().SetBool(PlayerAnimation.isIdleBool, true);
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        dashControl = Runner.GetDashControls();
        attackControl = Runner.GetAttackControls();
        mobilityControl = Runner.GetMobilityControl();

        // FIXME: FIGURE OUT WHY ATTACK AND DASH CONTROLS NOT BEING PASSED TO SUB STATES
    }

    public override void CheckStateTransition()
    {
        if (dashControl > 0){
            CurrentSuperState.SetSubState(typeof(SprintDashState));
        } 
        // else if (attackControl > 0){
        //     CurrentSuperState.SetSubState(typeof(GroundSubAttackOne));
        // } // TODO: Think of how to do attacking state while sprinting
        else if (verticalControl > 0){
            CurrentSuperState.SetSubState(typeof(SprintJumpState));
        }
        else if (!Runner.GetGroundCheck().Check() && (verticalControl < 0 || Runner.GetRigidbody2D().velocity.y < 0)){
            CurrentSuperState.SetSubState(typeof(SprintFallCoyoteState));
        }
        else if (mobilityControl > 0) {
            CurrentSuperState.SetSubState(typeof(SprintSlideState));
        }
        // Last Run to run as it's a high control function
        else if (horizontalControl != 0){
            CurrentSuperState.SetSubState(typeof(SprintRunState));
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isIdleBool, false);
        yield break;
    }

    public override void UpdateState()
    {
        Runner.GetRigidbody2D().velocity = new Vector2(0, Runner.GetRigidbody2D().velocity.y);
    }
}