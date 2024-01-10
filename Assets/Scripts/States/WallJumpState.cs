using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Wall Jump")]
public class WallJumpState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl;
    private Rigidbody2D rb2d;
    private bool canMove;
    private Coroutine currentWallDelay;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        rb2d = parent.GetRigidbody2D();

        if (currentWallDelay != null){
            Runner.StopCoroutine(currentWallDelay);
            currentWallDelay = null;
        }
        canMove = false;
        float wallJumpDirection = -Runner.transform.localScale.x;
        Vector2 jumpForce = new Vector2(wallJumpDirection * Runner.GetPlayerData().wallJumpForce.x, Runner.GetPlayerData().wallJumpForce.y);
        Debug.Log(jumpForce);
        rb2d.AddForce(jumpForce, ForceMode2D.Impulse);
        Debug.Log(rb2d.gravityScale);
        Debug.Log(rb2d.velocity);

        Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerWallJump);
        Runner.GetAnimator().SetBool(PlayerAnimation.isWallJumpingBool, true);

        if (Runner.transform.localScale.x * wallJumpDirection <= 0){
            Vector3 localScale = Runner.transform.localScale;
            localScale.x *= -1f;
            Runner.transform.localScale = localScale;
        }

        if (!canMove){
            currentWallDelay = Runner.StartCoroutine(WallJumpDelay());
        }
    }

    private IEnumerator WallJumpDelay(){
        yield return new WaitForSeconds(Runner.GetPlayerData().wallJumpDuration);
        Debug.Log(rb2d.velocity);
        canMove = true;
    }

    public override void CaptureInput()
    {   
        verticalControl = Runner.GetVerticalControls();
        horizontalControl = Runner.GetHorizontalControls();
        dashControl = Runner.GetDashControls();
    }

    public override void CheckStateTransition()
    {

        // FIXME: If Crashing into top wall, velocity will drop to 0 hence becomes falling state
        if (canMove){
            if (dashControl > 0){
                Runner.SetMainState(typeof(DashState));
            }
            else if (verticalControl <= 0 || rb2d.velocity.y <= 0){
                Runner.SetMainState(typeof(FallState));
            }
        }
        else if (Runner.GetWallCheck().Check()){
            Runner.SetMainState(typeof(WallClingState));
        }
    }

    public override void ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isWallJumpingBool, false);
        currentWallDelay = null;
    }

    public override void FixedUpdateState()
    {
        if (canMove && horizontalControl != 0){
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
}
