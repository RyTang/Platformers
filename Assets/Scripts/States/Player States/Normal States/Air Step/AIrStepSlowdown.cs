using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Air Step/Slow down")]
public class AirStepSlowdown : BaseState<PlayerController>
{
    private float verticalControl, horizontalControl, jumpControl;

    private Vector2 airStepDirection;

    private Coroutine slowdownCoroutine;
    private bool slowModeActive = false;

    private bool letGoOfJump = false;

    private float durationInSlowMode = 0f;

    public override void CaptureInput()
    {
        verticalControl = Runner.GetVerticalControl();
        horizontalControl = Runner.GetHorizontalControl();
        jumpControl = Runner.GetJumpControl();

        airStepDirection = new Vector2(horizontalControl, verticalControl).normalized;

        if (jumpControl <= 0)
        {
            letGoOfJump = true;
        }

        // TODO: Decide to hold slow mo to charge jump or not, or do we need a slow down stage at all
    }

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        // TODO: Figure out how to make the slow mo time smoother

        Debug.Log("Entered Air Step Slowdown");

        // Slow down Time during Air Step Slowdown
        letGoOfJump = false;
        durationInSlowMode = 0;
        GameManager.SetTimeScale(Runner.GetPlayerData().airStepSlowdownFactor);
        Debug.Log($"Slowing down time to {Time.timeScale}");

        slowdownCoroutine = Runner.StartCoroutine(SlowdownDuration());
    }

    public IEnumerator SlowdownDuration()
    {
        slowModeActive = true;
        yield return new WaitForSecondsRealtime(Runner.GetPlayerData().maxAirStepSlowdownDuration);
        slowModeActive = false;
    }

    public override void CheckStateTransition()
    {
        // Must have a direction to have an Air Step
        if (airStepDirection != Vector2.zero && (!slowModeActive || letGoOfJump))
        {
            float powerFactor = Mathf.Clamp01(durationInSlowMode / Runner.GetPlayerData().airStepThresholdDuration);
            Debug.Log($"Air Step Direction: {airStepDirection}, Power Factor: {powerFactor}");

            AirStepData airStepData = new AirStepData(airStepDirection, powerFactor);
            CurrentSuperState.SetSubState(typeof(AirStepState), airStepData);
            // TODO: Calculate force based on duration in slow mo
        }
        else if (!Runner.GetGroundCheck().Check() && (!slowModeActive || letGoOfJump))
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
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check())
        {
            CurrentSuperState.SetSubState(typeof(NormalWallClingState));
        }
    }

    public override void UpdateState()
    {
        Debug.Log($"In Slowdown State, timeScale: {Time.timeScale}, durationInSlowMode: {durationInSlowMode}");
        durationInSlowMode += Time.unscaledDeltaTime;

        base.UpdateState();
    }

    public override IEnumerator ExitState()
    {
        slowModeActive = false;
        if (slowdownCoroutine != null)
        {
            Runner.StopCoroutine(slowdownCoroutine);
            slowdownCoroutine = null;
        }
        GameManager.SetTimeScale(1f);
        return base.ExitState();
    }
    
}