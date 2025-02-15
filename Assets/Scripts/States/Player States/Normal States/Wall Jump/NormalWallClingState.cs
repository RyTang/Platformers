using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Wall Cling")]
public class NormalWallClingState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl;
    private Rigidbody2D rb2d;

    private bool canJump = false;

    private Coroutine clingDelay;

    public override void EnterState(PlayerController parent)
    {

        base.EnterState(parent);
        rb2d = parent.GetRigidbody2D();

        if (clingDelay != null){
            Runner.StopCoroutine(clingDelay);
        }

        canJump = false;

        Runner.GetAnimator().SetBool(PlayerAnimation.isWallClingingBool, true);
        clingDelay = Runner.StartCoroutine(ClingDelay());
    }


    public override void CaptureInput()
    {
        verticalControl = Runner.GetVerticalControls();
        horizontalControl = Runner.GetHorizontalControls();
        dashControl = Runner.GetDashControls();
    }

    public override void CheckStateTransition()
    {
        if (dashControl > 0 && Runner.GetWallCheck().Check()){
            Vector3 localScale = Runner.transform.localScale;
            localScale.x *= -1f;
            Runner.transform.localScale = localScale;
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalDashState)));
        }
        // In the case of not in contact with wall 
        else if (!Runner.GetWallCheck().Check() || (Mathf.Sign(horizontalControl) != Mathf.Sign(Runner.transform.localScale.x) && horizontalControl != 0)){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFallCoyoteState)));
        }
        else if (verticalControl > 0 && canJump){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalWallJumpState)));
        }
        else if (Runner.GetLedgeCheck().Check()) {
            CurrentSuperState.SetSubState(typeof(NormalLedgeHangState));
        }
        else if (verticalControl < 0) {
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFallState)));
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
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalLandState)), collision.relativeVelocity.y);
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
