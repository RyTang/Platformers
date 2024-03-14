using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Coyote Fall")]
public class NormalFallCoyoteState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl;

    private float initialLocalGravity;

    Rigidbody2D rb2d;

    private bool stillCoyote;

    private Coroutine coyoteTimer;


    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        rb2d = parent.GetRigidbody2D();
        initialLocalGravity = rb2d.gravityScale;

        stillCoyote = true;
        coyoteTimer = Runner.StartCoroutine(CoyoteTimer());


        rb2d.gravityScale = initialLocalGravity * Runner.GetPlayerData().fallGravityMultiplier;

        Runner.GetAnimator().SetBool(PlayerAnimation.isFallingBool, true);

        // TODO: Look into doing coyote Timing here. Need to figure out how to differentiate Wall Jump vs Jump
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        dashControl = Runner.GetDashControls();
    }

    public override void CheckStateTransition()
    {   
        if (!stillCoyote){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFallState)));
        }
        else if (verticalControl > 0 && stillCoyote){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalJumpState)));
        }
        else if (dashControl > 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalDashState)));
        }
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalWallClingState)));
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
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalLandState)), collision.relativeVelocity.y);
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

    public IEnumerator CoyoteTimer(){
        yield return new WaitForSeconds(Runner.GetPlayerData().coyoteTime);
        stillCoyote = false;
        coyoteTimer = null;
    }
    
    public override void OnStateCollisionStay(Collision2D collision)
    {
    }

    public override void OnStateCollisionExit(Collision2D collision)
    {
    }
}