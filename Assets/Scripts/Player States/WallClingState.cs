using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Wall Cling")]
public class WallClingState : BaseState<PlayerController>
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
            Runner.SetMainState(typeof(DashState));
        }
        
        else if (!Runner.GetWallCheck().Check() || (horizontalControl != Runner.transform.localScale.x && horizontalControl != 0)){
            Runner.SetMainState(typeof(FallState));
        }
        else if (Runner.GetGroundCheck().Check()){
            Runner.SetMainState(typeof(LandState), Runner.GetRigidbody2D().velocity.y);
        }
        else if (verticalControl > 0 & canJump){
            Runner.SetMainState(typeof(WallJumpState));
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
    }

    public override void UpdateState()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Clamp(rb2d.velocity.y, -Runner.GetPlayerData().wallSlidingSpeed, Runner.GetPlayerData().wallSlidingSpeed));
    }

    public IEnumerator ClingDelay(){
        yield return new WaitForSeconds(Runner.GetPlayerData().clingDelay);
        canJump = true;
    }

    public IEnumerator CoyoteTimer(){
        yield return new WaitForSeconds(Runner.GetPlayerData().coyoteTime);
        canJump = false;
    }


    public override void InitialiseSubState()
    {
    }
}
