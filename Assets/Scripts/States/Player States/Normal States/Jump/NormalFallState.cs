using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Fall")]
public class NormalFallState : BaseState<PlayerController>
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
        rb2d.gravityScale = initialLocalGravity * Runner.GetPlayerData().fallGravityMultiplier;

        Runner.GetAnimator().SetBool(PlayerAnimation.isFallingBool, true);

    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        dashControl = Runner.GetDashControls();
        attackControl = Runner.GetAttackControls();
    }

    public override void CheckStateTransition()
    {   
        if (attackControl > 0) {
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalJumpAttack)));
        }
        else if (dashControl > 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalDashState)));
        }
        else if (Runner.GetLedgeCheck().Check()) {
            CurrentSuperState.SetSubState(typeof(NormalLedgeHangState));
        }
        else if ((rb2d.velocity.x != 0 || horizontalControl != 0) && Runner.GetWallCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalWallClingState)));
        }
        else if (verticalControl < 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFreeFallState)));
        }
    }

    public override IEnumerator ExitState()
    {
        rb2d.gravityScale = initialLocalGravity;
        Runner.GetAnimator().SetBool(PlayerAnimation.isFallingBool, false);
        yield break;
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
        if (Runner.GetGroundCheck().Check() && !(Runner.GetWallCheck().Check() && rb2d.velocity.x != 0)) { // Problem Statement: Sometimes it will trigger the Land State when falling and colliding with the fall whereas it should have been a wall Cling State
            CurrentSuperState.SetSubState(typeof(NormalLandState), collision.relativeVelocity.y);
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


    public override void OnStateCollisionStay(Collision2D collision)
    {
        if (Runner.GetGroundCheck().Check()){
            Debug.Log($"TouchedGround: {collision.relativeVelocity}");
            CurrentSuperState.SetSubState(typeof(NormalLandState), collision.relativeVelocity.y);
        }
    }

    public override void FixedUpdateState()
    {
    }
}