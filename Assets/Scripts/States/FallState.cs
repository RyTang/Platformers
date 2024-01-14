using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Fall")]
public class FallState : BaseState<PlayerController>
{
    private float horizontalControl, dashControl;

    private float initialLocalGravity;

    Rigidbody2D rb2d;

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
        dashControl = Runner.GetDashControls();
    }

    public override void CheckStateTransition()
    {   
        if (dashControl > 0){
            Runner.SetMainState(typeof(DashState));
        }
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check()){
            Runner.SetMainState(typeof(WallClingState));
        }
    }

    public override IEnumerator ExitState()
    {
        rb2d.gravityScale = initialLocalGravity;
        Runner.GetAnimator().SetBool(PlayerAnimation.isFallingBool, false);
        yield break;
    }

    public override void FixedUpdateState()
    {

    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
        if (Runner.GetGroundCheck()){
            Runner.SetMainState(typeof(LandState), collision.relativeVelocity.y);
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
}