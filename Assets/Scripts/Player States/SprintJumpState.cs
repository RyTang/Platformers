using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Sprint Jump State")]
public class SprintJumpState : BaseState<PlayerController>
{
    Rigidbody2D rb2d;

    private float horizontalControl, verticalControl, dashControl;

    private bool canMove;
    private Coroutine currentSprintJumpDelay;

    public override void EnterState(PlayerController parent, float floatToPass)
    {
        base.EnterState(parent, floatToPass);
        rb2d = Runner.GetRigidbody2D();

        if (currentSprintJumpDelay != null) {
            Runner.StopCoroutine(currentSprintJumpDelay);
            currentSprintJumpDelay = null;
        }
        
        canMove = false;
        float xVelocity = Mathf.Clamp(floatToPass * Runner.GetPlayerData().sprintJumpMultiplier, 0, Runner.GetPlayerData().maxSprintJumpVelocity);
        Vector2 jumpVector = new Vector2(xVelocity, Runner.GetPlayerData().sprintJumpHeight);
        
        rb2d.AddForce(jumpVector, ForceMode2D.Impulse);
        
        
        Runner.GetAnimator().SetBool(PlayerAnimation.isJumping, true);
        Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerJump);

        currentSprintJumpDelay = Runner.StartCoroutine(SprintJumpDelay());
    }

    
    private IEnumerator SprintJumpDelay(){
        yield return new WaitForSeconds(Runner.GetPlayerData().sprintJumpDuration);
        canMove = true;
    }


    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        dashControl = Runner.GetDashControls();
    }

    public override void CheckStateTransition()
    {
        if (canMove){
            if (dashControl > 0){
            Runner.SetMainState(typeof(DashState));
            }
            else if (verticalControl <= 0 || rb2d.velocity.y <= 0){
                Runner.SetMainState(typeof(FallState));
            }
        }        
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check()){
            Runner.SetMainState(typeof(WallClingState));
        } 
    }
    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isJumping, false);
        currentSprintJumpDelay = null;
        yield break;
    }

    public override void FixedUpdateState()
    {
        if (canMove && horizontalControl != 0){
            rb2d.velocity = horizontalControl > 0 ? new Vector2(Runner.GetPlayerData().moveSpeed, rb2d.velocity.y) : new Vector2(-Runner.GetPlayerData().moveSpeed, rb2d.velocity.y);
        }
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
        if (Runner.GetGroundCheck()){
            Runner.SetMainState(typeof(LandState), collision.relativeVelocity.y);
        }
    }

    public override void UpdateState()
    {
    }

    public override void InitialiseSubState()
    {
    }
}
