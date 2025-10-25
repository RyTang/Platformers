using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Wall Cling")]
public class NormalWallClingState : BaseState<PlayerController>
{
    private float horizontalControl, jumpControl, dashControl, verticalControl;
    private Rigidbody2D rb2d;

    private bool canJump = false;

    private Coroutine clingDelay;

    private bool hasReleasedJump = false;

    public override void EnterState(PlayerController parent)
    {
        hasReleasedJump = false;

        base.EnterState(parent);
        rb2d = parent.GetRigidbody2D();

        if (clingDelay != null)
        {
            Runner.StopCoroutine(clingDelay);
        }

        canJump = false;

        Runner.GetAnimator().SetBool(PlayerAnimation.isWallClingingBool, true);
        clingDelay = Runner.StartCoroutine(ClingDelay());

        // Reset certian movement abilities
        Runner.RefreshAirStep();
    }


    public override void CaptureInput()
    {
        jumpControl = Runner.GetJumpControl();
        verticalControl = Runner.GetVerticalControl();
        horizontalControl = Runner.GetHorizontalControl();
        dashControl = Runner.GetDashControls();

        if (jumpControl <= 0)
        {
            hasReleasedJump = true;
        }
    }

    public override void CheckStateTransition()
    {
        float facingDirection = Runner.IsFacingRight() ? 1 : -1;
        if (dashControl > 0 && Runner.GetWallCheck().Check())
        {
            Runner.SetFacingDirection(!Runner.IsFacingRight());
            CurrentSuperState.SetSubState(typeof(NormalDashState), Mathf.Sign(Runner.IsFacingRight() ? 1 : -1));
            // FIXME: FIgure out how to deal with this interaction with Dash where direction input is read and changes direction of Dash into wall
        }
        // In the case of not in contact with wall 
        else if (!Runner.GetWallCheck().Check() || (Mathf.Sign(horizontalControl) != Mathf.Sign(facingDirection) && horizontalControl != 0))
        {
            CurrentSuperState.SetSubState(typeof(NormalFallCoyoteState));
        }
        else if (hasReleasedJump && jumpControl > 0 && canJump)
        {
            CurrentSuperState.SetSubState(typeof(NormalWallJumpState));
        }
        else if (Runner.GetHybridLedgeDetector().TryFindLedge(out _, out _, out _))
        {
            CurrentSuperState.SetSubState(typeof(NormalLedgeHangState));
        }
        else if (verticalControl < 0)
        {
            CurrentSuperState.SetSubState(typeof(NormalFallState));
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isWallClingingBool, false);
        yield break;
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
        if (Runner.GetGroundCheck().Check() && rb2d.velocity.y <= 0){
            CurrentSuperState.SetSubState(typeof(NormalLandState), collision.relativeVelocity.y);
        }
    }

    public override void UpdateState()
    {
        rb2d.velocity = new Vector2(0, Mathf.Clamp(rb2d.velocity.y, -Runner.GetPlayerData().wallSlidingSpeed, Runner.GetPlayerData().wallSlidingSpeed));
    }

    public IEnumerator ClingDelay(){
        yield return new WaitForSeconds(Runner.GetPlayerData().clingDelay);
        canJump = true;
    }

}
