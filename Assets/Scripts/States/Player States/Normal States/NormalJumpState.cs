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
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalJumpAttack)));
        }
        else if (dashControl > 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalDashState)));
        }
        else if (verticalControl <= 0 || rb2d.velocity.y <= 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFallState)));
        }
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalWallClingState)));
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

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void UpdateState()
    {
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