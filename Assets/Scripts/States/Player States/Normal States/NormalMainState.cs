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

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        
        InitialiseSubState();
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        sprintControl = Runner.GetSprintControls();
    }

    public override void CheckStateTransition()
    {
        // TODO: need to set a min energy recovery, if depletes the entire bar
    }

    public override void InitialiseSubState()
    {
        if ((verticalControl < 0 && !Runner.GetGroundCheck().Check()) || (Runner.GetRigidbody2D().velocity.y < 0 && !canJump)){
            SetSubState(GetState(typeof(NormalFallState)));
        }
        else if (verticalControl > 0 && Runner.GetGroundCheck().Check()){
            SetSubState(GetState(typeof(NormalJumpState)));
        }
        else if (horizontalControl != 0){
            if (sprintControl > 0 ){
                CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalSprintState)));
            }
            else {
                CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalRunState)));
            }
        }
        else {
            SetSubState(GetState(typeof(NormalIdleState)));
        }
    }
    
}