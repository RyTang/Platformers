using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Sprint State/Main State")]
public class SprintMainState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, sprintControl;
    private bool canJump = false;

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        sprintControl = Runner.GetSprintControls();
    }

    public override void InitialiseSubState()
    {
        if ((verticalControl < 0 && !Runner.GetGroundCheck().Check()) || (Runner.GetRigidbody2D().velocity.y < 0 && !canJump)){
            SetSubState(typeof(SprintFallState));
        }
        else if (verticalControl > 0 && Runner.GetGroundCheck().Check()){
            SetSubState(typeof(SprintJumpState));
        }
        else if (horizontalControl != 0){
            SetSubState(typeof(SprintRunState));
        }
        else {
            SetSubState(typeof(SprintIdleState));
        }
    }

    public override void CheckStateTransition()
    {
        if (sprintControl <= 0) {
            Runner.SetMainState(typeof(NormalMainState));
        }
    }
}