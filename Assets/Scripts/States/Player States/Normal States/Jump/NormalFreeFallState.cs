using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Free Fall")]
public class NormalFreeFallState : BaseState<PlayerController>
{
    private float horizontalControl, dashControl, attackControl, verticalControl;
    private float initialLocalGravity;
    private bool diving = false;
    Rigidbody2D rb2d;

    PlayerController parent;

    public override void EnterState(PlayerController parent)
    {
        this.parent = parent;
        this.parent.AnimationEvent += OnAnimationEventTriggered;
        base.EnterState(parent);

        rb2d = parent.GetRigidbody2D();

        initialLocalGravity = rb2d.gravityScale;

        // Momentarrilly Pause Mid Air
        // TODO: By doing this need to prevent users from spamming this interaction
        // rb2d.gravityScale = 0f;
        // rb2d.velocity = Vector2.zero;
        diving = false;
        Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerFreefall);
        Runner.GetAnimator().SetBool(PlayerAnimation.isFreeFallingBool, true);
    }

    private void OnAnimationEventTriggered(AnimationEventTrigger eventTrigger){
        if (eventTrigger == AnimationEventTrigger.START_DIVING){
            rb2d.gravityScale = initialLocalGravity * Runner.GetPlayerData().freeFallGravityMultiplier;
            diving = true;
            Debug.Log("Event Triggered");
        }
    }

    public override void CaptureInput()
    {
        verticalControl = Runner.GetVerticalControls();
        horizontalControl = Runner.GetHorizontalControls();
        dashControl = Runner.GetDashControls();
        attackControl = Runner.GetAttackControls();
    }

    public override IEnumerator ExitState()
    {
        
        rb2d.gravityScale = initialLocalGravity;
        Runner.GetAnimator().SetBool(PlayerAnimation.isFreeFallingBool, false);
        diving = false;
        parent.AnimationEvent -= OnAnimationEventTriggered;
        return base.ExitState();
    }

    public override void CheckStateTransition()
    {
        if (attackControl > 0) {
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalJumpAttack)));
        }
        else if (dashControl > 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalDashState)));
        }
        else if (verticalControl >= 0 && horizontalControl != 0 && Runner.GetWallCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalWallClingState)));
        }
        else if (verticalControl >= 0) {
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFallState)));
        }
    }


    public override void UpdateState()
    {
        float x_velocity = 0;
        float y_velocity = rb2d.velocity.y;

        if (horizontalControl != 0){
            x_velocity = horizontalControl > 0 ? Runner.GetPlayerData().horizontalFreeFallSpeed : -Runner.GetPlayerData().horizontalFallSpeed;
        }
        
        // TODO: Change this to have have higher Terminal Velocity and increased Gravity Speed
        if (diving){
            y_velocity = Mathf.Clamp(rb2d.velocity.y, -Runner.GetPlayerData().terminalFreeFallSpeed, float.MaxValue);
        }
        rb2d.velocity = new Vector2(x_velocity, y_velocity);
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
        if (Runner.GetGroundCheck().Check()){
            Debug.Log($"TouchedGround: {collision.relativeVelocity}");
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFreeFallLandState)), collision.relativeVelocity.y);
        }
    }

    public override void OnStateCollisionStay(Collision2D collision)
    {
        if (Runner.GetGroundCheck().Check()){
            Debug.Log($"TouchedGround: {collision.relativeVelocity}");
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFreeFallLandState)), collision.relativeVelocity.y);
        }
    }
}
