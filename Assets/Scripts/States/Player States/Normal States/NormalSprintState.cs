using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Sprint")]
public class NormalSprintState : BaseState<PlayerController>
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
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalRunState)));
        }
        else if (dashControl > 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalDashState)));
        }
        else if (attackControl > 0) {
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(GroundSubAttackOne)));
        }
        else if (horizontalControl == 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalIdleState)));
        }
        else if (verticalControl > 0) {
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalJumpState)));
        }
        else if (!Runner.GetGroundCheck().Check() && (verticalControl < 0 || Runner.GetRigidbody2D().velocity.y < 0)){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFallCoyoteState)));
        }
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalWallClingState)));
        }
        else if (mobilityControl > 0) {
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalSlideState)));
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