using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Player State/Normal State/Main State")]
public class NormalMainState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl, attackControl, sprintControl;
    
    private bool canJump = false;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        IsRootState = true;
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
        if (sprintControl > 0){
            Runner.SetMainState(typeof(SpeedMainState));
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void InitialiseSubState()
    {
        if (dashControl > 0){
            SetSubState(Runner.GetState(typeof(NormalDashState)));
        } 
        else if (attackControl > 0){
            SetSubState(Runner.GetState(typeof(GroundSubAttackOne)));
        }
        else if (horizontalControl != 0){
            SetSubState(Runner.GetState(typeof(NormalRunState)));
        }
        else if (verticalControl > 0 && Runner.GetGroundCheck().Check()){
            SetSubState(Runner.GetState(typeof(NormalJumpState)));
        }
        else if ((verticalControl < 0 && !Runner.GetGroundCheck().Check()) || (Runner.GetRigidbody2D().velocity.y < 0 && !canJump)){
            SetSubState(Runner.GetState(typeof(NormalFallState)));
        }
        else {
            SetSubState(Runner.GetState(typeof(NormalIdleState)));
        }
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void UpdateState()
    {        
    }
    
    public override void OnStateCollisionStay(Collision2D collision)
    {
    }

    public override void OnStateCollisionExit(Collision2D collision)
    {
    }
}