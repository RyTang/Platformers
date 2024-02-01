using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Player State/Speed State/Wall Jump State")]
public class SpeedWallJumpState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl;
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

        Vector2 normalWallJumpForce = Runner.GetPlayerData().wallJumpForce;
        Vector2 sprintWallJumpForceCap = normalWallJumpForce * Runner.GetPlayerData().sprintWallJumpMultiplier;
        Debug.Log("Sprint Wall Jump Cap: " + sprintWallJumpForceCap);

        float initialMagnitude = normalWallJumpForce.magnitude;
        initialMagnitude = rb2d.velocity.y > 0 ? initialMagnitude + rb2d.velocity.magnitude : initialMagnitude;
        

        Vector2 jumpForce = initialMagnitude * normalWallJumpForce.normalized;

        // TODO: Need to figure out if a Player Speed Cap is needed
        
        jumpForce.x = wallJumpDirection * jumpForce.x;

        jumpForce.x = Mathf.Clamp(jumpForce.x, -sprintWallJumpForceCap.x, sprintWallJumpForceCap.x);
        jumpForce.y = Mathf.Clamp(jumpForce.y, -sprintWallJumpForceCap.y, sprintWallJumpForceCap.y);
        
        Debug.Log("Sprint Wall Jump Force: " + jumpForce);

        // TODO: NEED TO CHANGE TO ACCOMODATE FORCES
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(jumpForce, ForceMode2D.Impulse);

        Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerWallJump);
        Runner.GetAnimator().SetBool(PlayerAnimation.isWallJumpingBool, true);

        if (Runner.transform.localScale.x * wallJumpDirection <= 0){
            Vector3 localScale = Runner.transform.localScale;
            localScale.x *= -1f;
            Runner.transform.localScale = localScale;
        }

        currentWallDelay = Runner.StartCoroutine(WallJumpDelay());
    }

    private IEnumerator WallJumpDelay(){
        yield return new WaitForSeconds(Runner.GetPlayerData().wallJumpDuration);
        canMove = true;
    }

    public override void CaptureInput()
    {   
        verticalControl = Runner.GetVerticalControls();
        horizontalControl = Runner.GetHorizontalControls();
    }

    public override void CheckStateTransition()
    {
        // FIXME: If Crashing into top wall, velocity will drop to 0 hence becomes falling state
        if (canMove){
            if (verticalControl <= 0 || rb2d.velocity.y <= 0){
                CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedFallState)));
            }
        }
        else if (Runner.GetWallCheck().Check()){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedWallClingState)));
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isWallJumpingBool, false);
        currentWallDelay = null;
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