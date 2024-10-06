using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Wall Jump")]
public class NormalWallJumpState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl, attackControl;
    private PlayerController parent;
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

        // Change Direction of the Jump to face the right direction
        canMove = false;
        this.parent = parent;
        this.parent.canRotate = false;
        float wallJumpDirection = -Runner.transform.localScale.x;
        Vector2 jumpForce = new Vector2(wallJumpDirection * Runner.GetPlayerData().wallJumpForce.x, Runner.GetPlayerData().wallJumpForce.y);
        rb2d.AddForce(jumpForce, ForceMode2D.Impulse);

        if (Runner.transform.localScale.x * wallJumpDirection <= 0){
            Vector3 localScale = Runner.transform.localScale;
            localScale.x *= -1f;
            Runner.transform.localScale = localScale;
        }

        Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerWallJump);
        Runner.GetAnimator().SetBool(PlayerAnimation.isWallJumpingBool, true);

        

        if (!canMove){
            currentWallDelay = Runner.StartCoroutine(WallJumpDelay());
        }
    }

    private IEnumerator WallJumpDelay(){
        yield return new WaitForSeconds(Runner.GetPlayerData().wallJumpDuration);
        canMove = true;
        
    }

    public override void CaptureInput()
    {   
        if (canMove){
            parent.canRotate = true;
            verticalControl = Runner.GetVerticalControls();
            horizontalControl = Runner.GetHorizontalControls();
            dashControl = Runner.GetDashControls();
            attackControl = Runner.GetAttackControls();
        }
    }

    public override void CheckStateTransition()
    {

        // Transition to a jump state for some reason;
        if (canMove){
            if (dashControl > 0){
                CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalDashState)));
            }
            else if (verticalControl <= 0 || rb2d.velocity.y <= 0){
                CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFallState)));
            }
            else if (attackControl > 0) {
               CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalJumpAttack)));
            }
        }
        if (Runner.GetWallCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalWallClingState))); // FIXME: Seems to be an Issue where If wall jumping without pressing direction, it won't trigger the wall cling state
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isWallJumpingBool, false);
        currentWallDelay = null;
        parent.canRotate = true;
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
        if (Runner.GetGroundCheck().Check() && !(Runner.GetWallCheck().Check() && rb2d.velocity.x != 0)) { // Problem Statement: Sometimes it will trigger the Land State when falling and colliding with the fall whereas it should have been a wall Cling State
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalLandState)), collision.relativeVelocity.y);
        }
    }
}
