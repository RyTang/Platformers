using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Speed State/Jump State")]
public class SpeedJumpState : BaseState<PlayerController>
{
    Rigidbody2D rb2d;

    private float horizontalControl, verticalControl;

    private bool canMove;
    private Coroutine currentSprintJumpDelay;

    public override void EnterState(PlayerController parent, object objToPass)
    {
        base.EnterState(parent, objToPass);
        rb2d = Runner.GetRigidbody2D();

        if (currentSprintJumpDelay != null) {
            Runner.StopCoroutine(currentSprintJumpDelay);
            currentSprintJumpDelay = null;
        }
        
        canMove = false;
        float xVelocity = Mathf.Clamp((float) objToPass * Runner.GetPlayerData().sprintJumpMultiplier, 0, Runner.GetPlayerData().maxSprintJumpVelocity);
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
    }

    public override void CheckStateTransition()
    {
        if (verticalControl <= 0 || rb2d.velocity.y <= 0){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedFallState)));
        }     
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check()){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedWallClingState)));
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
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedLandState)), collision.relativeVelocity.y);
        }
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
