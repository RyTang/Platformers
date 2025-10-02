using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Air Step/Air Step")]
public class AirStepState : BaseState<PlayerController>
{
    private Rigidbody2D rb2d;

    private Vector2 airStepDirection;
    private AirStepData airStepData;

    private bool airStepInputGiven = false;
    private bool isAirStepping = false;

    private Coroutine airStepDurationCoroutine;

    

    private float verticalControl, horizontalControl, jumpControl;

    public override void CaptureInput()
    {
        // Get direction of Air Step from input
        verticalControl = Runner.GetVerticalControl();
        horizontalControl = Runner.GetHorizontalControl();
        jumpControl = Runner.GetJumpControl();
    }

    public override void EnterState(PlayerController parent, object objToPass)
    {
        airStepData = (AirStepData) objToPass;
        airStepDirection = airStepData.airStepDirection;
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

        rb2d.velocity = new Vector2(0, 0);
        rb2d.AddForce(airStepDirection.normalized * Runner.GetPlayerData().airStepForce * airStepData.powerFactor, ForceMode2D.Impulse);

        airStepDurationCoroutine = Runner.StartCoroutine(AirStepDuration());

    }

    public IEnumerator AirStepDuration(){
        yield return new WaitForSeconds(Runner.GetPlayerData().airStepDuration);
        isAirStepping = false;
    }


    public override void CheckStateTransition()
    {
        if (Runner.GetGroundCheck().Check())
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
        // TODO: Consider removing this variable jump thingy
        // If Jump Button not held down, then stop Air Step
        else if (!isAirStepping)
        {
            CurrentSuperState.SetSubState(typeof(NormalFallState));
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, false); // TODO: Change this
        isAirStepping = false;
        airStepInputGiven = false;
        if (airStepDurationCoroutine != null)
        {
            Runner.StopCoroutine(airStepDurationCoroutine);
            airStepDurationCoroutine = null;
        }

        return base.ExitState();
    }
}