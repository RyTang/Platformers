using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Player State/Speed State/Idle State")]
public class SpeedIdleState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, attackControl;
    
    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        Runner.GetAnimator().SetBool(PlayerAnimation.isIdleBool, true);
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        attackControl = Runner.GetAttackControls();

        // FIXME: FIGURE OUT WHY ATTACK AND DASH CONTROLS NOT BEING PASSED TO SUB STATES
    }

    public override void CheckStateTransition()
    {
        if (attackControl > 0){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(GroundSubAttackOne)));
        }
        else if (horizontalControl != 0){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedRunState)));
        }
        else if (verticalControl > 0){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedJumpState)), Runner.GetRigidbody2D().velocity.x);
        }
        else if (!Runner.GetGroundCheck().Check() && (verticalControl < 0 || Runner.GetRigidbody2D().velocity.y < 0)){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedFallCoyoteState)));
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isIdleBool, false);
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
        Runner.GetRigidbody2D().velocity = new Vector2(0, Runner.GetRigidbody2D().velocity.y);
    }

    public override void InitialiseSubState()
    {
    }

    public override void OnStateCollisionStay(Collision2D collision)
    {
    }

    public override void OnStateCollisionExit(Collision2D collision)
    {
    }
}