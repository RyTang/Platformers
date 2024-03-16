using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Idle")]
public class NormalIdleState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl, attackControl;
    
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

        // FIXME: FIGURE OUT WHY ATTACK AND DASH CONTROLS NOT BEING PASSED TO SUB STATES
    }

    public override void CheckStateTransition()
    {
        if (dashControl > 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalDashState)));
        } 
        else if (attackControl > 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(GroundSubAttackOne)));
        }
        else if (horizontalControl != 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalRunState)));
        }
        else if (verticalControl > 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalJumpState)));
        }
        else if (!Runner.GetGroundCheck().Check() && (verticalControl < 0 || Runner.GetRigidbody2D().velocity.y < 0)){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFallCoyoteState)));
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