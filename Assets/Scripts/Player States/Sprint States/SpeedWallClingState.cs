using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Player State/Speed State/Wall Cling State")]
public class SpeedWallClingState : BaseState<PlayerController>
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
        if (!Runner.GetWallCheck().Check() || (horizontalControl != Runner.transform.localScale.x && horizontalControl != 0)){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedFallCoyoteState)));
        }
        else if (verticalControl > 0 & canJump){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedWallJumpState)));
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isWallClingingBool, false);
        yield break;
    }

    public override void FixedUpdateState()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
        if (Runner.GetGroundCheck().Check()){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedLandState)), collision.relativeVelocity.y);
        }
    }

    public override void UpdateState()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Clamp(rb2d.velocity.y, -Runner.GetPlayerData().wallSlidingSpeed, Runner.GetPlayerData().wallSlidingSpeed));
    }

    public IEnumerator ClingDelay(){
        yield return new WaitForSeconds(Runner.GetPlayerData().clingDelay);
        canJump = true;
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