using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Player State/Normal State/Main State")]
public class NormalMainState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, sprintControl;
    
    private bool canJump = false;

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        sprintControl = Runner.GetSprintControls();
    }

    public override void CheckStateTransition()
    {
        // TODO: need to set a min energy recovery, if depletes the entire bar
        if (sprintControl > 0) {
            Runner.SetMainState(typeof(SprintMainState));
        }
    }

    public override void InitialiseSubState()
    {
        if ((verticalControl < 0 && !Runner.GetGroundCheck().Check()) || (Runner.GetRigidbody2D().velocity.y < 0 && !canJump)){
            SetSubState(typeof(NormalFallState));
        }
        else if (verticalControl > 0 && Runner.GetGroundCheck().Check()){
            SetSubState(typeof(NormalJumpState));
        }
        else if (horizontalControl != 0){
            SetSubState(typeof(NormalRunState));
        }
        else {
            SetSubState(typeof(NormalIdleState));
        }
    }

}