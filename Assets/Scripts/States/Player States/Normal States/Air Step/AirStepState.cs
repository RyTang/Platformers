using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Air Step")]
public class AirStepState : BaseState<PlayerController>
{
    private Rigidbody2D rb2d;

    private Vector2 airStepDirection;

    private bool airStepInputGiven = false;
    private bool isAirStepping = false;

    

    private float verticalControl, horizontalControl, jumpControl;

    public override void CaptureInput()
    {
        // Get direction of Air Step from input
        verticalControl = Runner.GetVerticalControl();
        horizontalControl = Runner.GetHorizontalControl();
        jumpControl = Runner.GetJumpControl();

        // airStepDirection = new Vector2(horizontalControls, verticalControls).normalized;
    }

    public override void EnterState(PlayerController parent, object objToPass)
    {
        Debug.Log("Air Stepping");
        airStepDirection = ((Vector2)objToPass).normalized; // Ensure Vector is normalized for direction input
        airStepInputGiven = true;
        base.EnterState(parent, objToPass);
    }

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        rb2d = Runner.GetRigidbody2D();


        if (!airStepInputGiven || airStepDirection == Vector2.zero)
        {
            airStepDirection = Vector2.up; // Default to upwards if no input, normal jump
        }

        isAirStepping = true;
        Runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, true); // TODO: Change this
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(airStepDirection * Runner.GetPlayerData().airStepForce, ForceMode2D.Impulse); // TOOD Need to make magnitude is clamped
        // TODO: Need to fix horizontal forces

    }


    public override void CheckStateTransition()
    {
        if (!isAirStepping && !Runner.GetGroundCheck().Check())
        {
            CurrentSuperState.SetSubState(typeof(NormalFallState));
        }
        else if (Runner.GetGroundCheck().Check())
        {
            CurrentSuperState.SetSubState(typeof(NormalIdleState));
        }
        else if (Runner.GetLedgeCheck().Check())
        {
            CurrentSuperState.SetSubState(typeof(NormalLedgeHangState));
        }
        else if (Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check())
        {
            CurrentSuperState.SetSubState(typeof(NormalWallClingState));
        }
        // If Jump Button not held down, then stop Air Step
        // TODO: Consider if this is good or not
        else if (jumpControl <= 0 || rb2d.velocity.y <= 0)
        {
            // TODO: if stop pressing should stop accelerating
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            CurrentSuperState.SetSubState(typeof(NormalFallState));
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, false); // TODO: Change this
        isAirStepping = false;
        airStepInputGiven = false;
        return base.ExitState();
    }


    // TODO: possibly reduce the motion movement for air step
    public override void FixedUpdateState()
    {
        // // TODO: How to 
        // if (horizontalControl == 0)
        // {
        //     rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        // }
        // else if (horizontalControl != 0)
        // {
        //     rb2d.velocity = horizontalControl > 0 ? new Vector2(Runner.GetPlayerData().moveSpeed, rb2d.velocity.y) : new Vector2(-Runner.GetPlayerData().moveSpeed, rb2d.velocity.y);
        // }
    }
}