using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Player State/Speed State/Fall State")]
public class SpeedFallState : BaseState<PlayerController>
{
    private float initialLocalGravity;

    Rigidbody2D rb2d;

    private float xVelocity;


    private Coroutine coyoteTimer;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        rb2d = parent.GetRigidbody2D();
        initialLocalGravity = rb2d.gravityScale;

        xVelocity = rb2d.velocity.x;

        rb2d.gravityScale = initialLocalGravity * Runner.GetPlayerData().fallGravityMultiplier;

        Runner.GetAnimator().SetBool(PlayerAnimation.isFallingBool, true);
    }

    public override void CaptureInput()
    {
    }

    public override void CheckStateTransition()
    {   
        if (Runner.GetWallCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SpeedWallClingState)));
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
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SpeedLandState)), collision.relativeVelocity.y);
        }
    }

    public override void UpdateState()
    {
        xVelocity -= Runner.GetPlayerData().attackDamage * Time.deltaTime;

        xVelocity = Mathf.Clamp(xVelocity, Runner.GetPlayerData().minSprintXFallSpeed, Mathf.Infinity);
    }


    public override void InitialiseSubState()
    {
    }

    public override void OnStateCollisionStay(Collision2D collision)
    {
        if (Runner.GetGroundCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SpeedLandState)), collision.relativeVelocity.y);
        }
    }

    public override void OnStateCollisionExit(Collision2D collision)
    {
    }
}