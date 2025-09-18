using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Sprint State/Run")]
public class SprintRunState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl, attackControl, mobilityControl;
    private float sprintControl;
    private Rigidbody2D rb2d;


    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        rb2d = parent.GetRigidbody2D();
        Runner.GetAnimator().SetBool(PlayerAnimation.isSprintingBool, true);
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
        if (sprintControl <= 0 && horizontalControl != 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SprintRunState)));
        }
        else if (dashControl > 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SprintDashState)));
        }
        // else if (attackControl > 0) {
        //     CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(GroundSubAttackOne)));
        // } // TODO: Think of how to do attacking state while sprinting
        else if (horizontalControl == 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SprintIdleState)));
        }
        else if (verticalControl > 0) {
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SprintJumpState)));
        }
        else if (!Runner.GetGroundCheck().Check() && (verticalControl < 0 || Runner.GetRigidbody2D().velocity.y < 0)){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SprintFallCoyoteState)));
        }
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SprintWallClingState)));
        }
        else if (mobilityControl > 0) {
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SprintSlideState)));
        }
    }
    
    public override IEnumerator ExitState(){
        Runner.GetAnimator().SetBool(PlayerAnimation.isSprintingBool, false);
        yield break;
    }

    public override void UpdateState()
    {
        if (horizontalControl != 0){
            rb2d.velocity = horizontalControl > 0 ? new Vector2(Runner.GetPlayerData().sprintState, rb2d.velocity.y) : new Vector2(-Runner.GetPlayerData().sprintState, rb2d.velocity.y);
        }
        
    }
}