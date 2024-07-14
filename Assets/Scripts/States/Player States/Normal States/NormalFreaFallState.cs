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
    Rigidbody2D rb2d;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        rb2d = parent.GetRigidbody2D();

        initialLocalGravity = rb2d.gravityScale;

        // Increase Gravity
        rb2d.gravityScale = initialLocalGravity * Runner.GetPlayerData().freeFallGravityMultiplier;

        Runner.GetAnimator().SetBool(PlayerAnimation.isFreeFallingBool, true);
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
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalWallClingState)));
        }
        else if (verticalControl >= 0) {
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFallState)));
        }
    }


    public override void UpdateState()
    {
        float x_velocity = 0;

        if (horizontalControl != 0){
            x_velocity = horizontalControl > 0 ? Runner.GetPlayerData().horizontalFreeFallSpeed : -Runner.GetPlayerData().horizontalFallSpeed;
        }
        
        // TODO: Change this to have have higher Terminal Velocity and increased Gravity Speed
        rb2d.velocity = new Vector2(x_velocity, Mathf.Clamp(rb2d.velocity.y, -Runner.GetPlayerData().terminalFreeFallSpeed, float.MaxValue));
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
        if (Runner.GetGroundCheck().Check()){
            Debug.Log($"TouchedGround: {collision.relativeVelocity}");
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalLandState)), collision.relativeVelocity.y);
        }
    }

    public override void OnStateCollisionStay(Collision2D collision)
    {

        // FIXME: For some reason velocity is not being translated to the Landing State Properly
        if (Runner.GetGroundCheck().Check()){
            Debug.Log($"TouchedGround: {collision.relativeVelocity}");
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalLandState)), collision.relativeVelocity.y);
        }
    }
}
