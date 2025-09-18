using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Jump")]
public class NormalJumpState : BaseState<PlayerController>
{
    Rigidbody2D rb2d;

    private float horizontalControl, verticalControl, dashControl, attackControl;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        rb2d = Runner.GetRigidbody2D();
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(new Vector2(0f, Runner.GetPlayerData().jumpForce), ForceMode2D.Impulse);
        Runner.GetAnimator().SetBool(PlayerAnimation.isJumping, true);
        Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerJump);
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        dashControl = Runner.GetDashControls();
        attackControl = Runner.GetAttackControls();
    }

    public override void CheckStateTransition()
    {
        if (attackControl > 0) {
            CurrentSuperState.SetSubState(typeof(NormalJumpAttack));
        }
        else if (dashControl > 0){
            CurrentSuperState.SetSubState(typeof(NormalDashState));
        }
        else if (verticalControl <= 0 || rb2d.velocity.y <= 0){
            // TODO: if stop pressing should stop accelerating
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            CurrentSuperState.SetSubState(typeof(NormalFallState));
        }
        else if (Runner.GetLedgeCheck().Check()) {
            CurrentSuperState.SetSubState(typeof(NormalLedgeHangState));
        }
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check()){
            CurrentSuperState.SetSubState(typeof(NormalWallClingState));
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isJumping, false);
        yield break;
    }

    public override void FixedUpdateState()
    {
        if (horizontalControl == 0) {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }
        else if (horizontalControl != 0){
            rb2d.velocity = horizontalControl > 0 ? new Vector2(Runner.GetPlayerData().moveSpeed, rb2d.velocity.y) : new Vector2(-Runner.GetPlayerData().moveSpeed, rb2d.velocity.y);
        }
    }
}