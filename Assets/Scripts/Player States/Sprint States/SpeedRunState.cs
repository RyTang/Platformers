using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Speed State/Run State")]
public class SpeedRunState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, attackControl;

    private Rigidbody2D rb2d;


    private float currentVelocityDirection = 0;
    private float currentSpeed = 0.0f;
    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        rb2d = parent.GetRigidbody2D();
        Runner.GetAnimator().SetBool(PlayerAnimation.isRunningBool, true);

        horizontalControl = Runner.GetHorizontalControls();

        currentVelocityDirection = Mathf.Sign(rb2d.velocity.x);
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        attackControl = Runner.GetAttackControls();
    }

    public override void CheckStateTransition()
    {
        if (attackControl > 0) {
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedGroundSubAttackOne)));
        }
        else if (horizontalControl == 0){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedIdleState)));
        }
        else if (verticalControl > 0) {
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedJumpState)), Runner.GetRigidbody2D().velocity.x);
        }
        else if (!Runner.GetGroundCheck().Check() && (verticalControl < 0 || Runner.GetRigidbody2D().velocity.y < 0)){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedFallCoyoteState)));
        }
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check()){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedWallClingState)));
        }
    }
    
    public override IEnumerator ExitState(){
        Runner.GetAnimator().SetBool(PlayerAnimation.isRunningBool, false);
        yield break;
    }

    public override void FixedUpdateState()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
        
    }

    public override void UpdateState()
    {
        if (horizontalControl != 0){
            AccelerateTowards();
        }
    }

    private void AccelerateTowards(){       

        if (currentVelocityDirection != horizontalControl){
            currentSpeed = 0;
            currentVelocityDirection = horizontalControl;
        }
        else if (currentVelocityDirection == horizontalControl){
            currentSpeed += Runner.GetPlayerData().accelerationSpeed * Time.deltaTime;

            currentSpeed = Mathf.Clamp(currentSpeed, 0, Runner.GetPlayerData().maxSprintSpeed);
        }
        // TODO: Should make it so that it stops (like drifting) for a moment when shifting directions
        
        rb2d.velocity = new Vector2(currentSpeed * currentVelocityDirection, rb2d.velocity.y);
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