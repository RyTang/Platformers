using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Player State/Speed State/Fall State")]
public class SpeedFallState : BaseState<PlayerController>
{
    private float horizontalControl;

    private float initialLocalGravity;

    Rigidbody2D rb2d;


    private Coroutine coyoteTimer;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        rb2d = parent.GetRigidbody2D();
        initialLocalGravity = rb2d.gravityScale;



        rb2d.gravityScale = initialLocalGravity * Runner.GetPlayerData().fallGravityMultiplier;

        Runner.GetAnimator().SetBool(PlayerAnimation.isFallingBool, true);

        // TODO: Look into doing coyote Timing here. Need to figure out how to differentiate Wall Jump vs Jump
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
    }

    public override void CheckStateTransition()
    {   
        if (horizontalControl != 0 && Runner.GetWallCheck().Check()){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedWallClingState)));
        }
    }

    public override IEnumerator ExitState()
    {
        if (coyoteTimer != null){
            Runner.StopCoroutine(coyoteTimer);
            coyoteTimer = null;
        }
        rb2d.gravityScale = initialLocalGravity;
        Runner.GetAnimator().SetBool(PlayerAnimation.isFallingBool, false);
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
        float x_velocity = 0;

        if (horizontalControl != 0){
            x_velocity = horizontalControl > 0 ? Runner.GetPlayerData().horizontalFallSpeed : -Runner.GetPlayerData().horizontalFallSpeed;
        }
        
        rb2d.velocity = new Vector2(x_velocity, Mathf.Clamp(rb2d.velocity.y, -Runner.GetPlayerData().terminalFallSpeed, float.MaxValue));
    }


    public override void InitialiseSubState()
    {
    }

    public override void OnStateCollisionStay(Collision2D collision)
    {
        if (Runner.GetGroundCheck().Check()){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedLandState)), collision.relativeVelocity.y);
        }
    }

    public override void OnStateCollisionExit(Collision2D collision)
    {
    }
}