using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Dash")]
public class NormalDashState : BaseState<PlayerController>
{
    private Rigidbody2D rb2d;

    private Coroutine currentDashDelay, dashBufferCoroutine;

    private bool canDash = true;
    private bool dashing, dashInputGiven, dashBufferDone;
    private float dashDirection;

    public override void EnterState(PlayerController parent, object objToPass)
    {
        dashDirection = (float)objToPass;
        dashInputGiven = true;

        base.EnterState(parent, objToPass);
    }


    public override void EnterState(PlayerController parent)
    {
        // Disable to prevent Direction from affecting velocity in this case -> If dashing then should not control so easily
        base.EnterState(parent);
        Runner.CanRotate(false);
        Runner.DisableHorizontalControls();
        Runner.DisableVerticalControls();

        if (!canDash && currentDashDelay != null)
        {
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalIdleState)));
            return;
        }

        rb2d = Runner.GetRigidbody2D();


        if (currentDashDelay != null)
        {
            Runner.StopCoroutine(currentDashDelay);
            Runner.StopCoroutine(dashBufferCoroutine);
            currentDashDelay = null;
            dashBufferCoroutine = null;
        }
        if (!dashInputGiven)
        {
            dashDirection = Mathf.Clamp(Runner.transform.localScale.x, -1, 1);
        }

        dashing = true;
        Runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, true);

        canDash = false;
        currentDashDelay = Runner.StartCoroutine(DashCooldown());
        dashBufferCoroutine = Runner.StartCoroutine(DashBuffer());
    }

    /// <summary>
    /// Cooldown for Dash after use
    /// </summary>
    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(Runner.GetPlayerData().dashDuration);
        Runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, false);
        dashing = false;
        yield return new WaitForSeconds(Runner.GetPlayerData().dashCooldown);
        canDash = true;
    }

    private IEnumerator DashBuffer()
    {
        dashBufferDone = false;
        
        yield return new WaitForSeconds(Runner.GetPlayerData().dashDuration);
        dashBufferDone = true;
    }

    public override void CheckStateTransition()
    {
        if (!dashing && !Runner.GetGroundCheck().Check())
        {
            CurrentSuperState.SetSubState(typeof(NormalFallState));
        }
        else if (!dashing && Runner.GetGroundCheck().Check())
        {
            CurrentSuperState.SetSubState(typeof(NormalIdleState));
        }
        else if (Runner.GetLedgeCheck().Check())
        {
            CurrentSuperState.SetSubState(typeof(NormalLedgeHangState));
        }
        else if (dashBufferDone && Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check())
        {
            Runner.StopCoroutine(currentDashDelay);
            currentDashDelay = null;
            Runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, false);
            canDash = true;
            CurrentSuperState.SetSubState(typeof(NormalWallClingState));
        }
    }

    public override IEnumerator ExitState()
    {
        dashing = false;
        dashInputGiven = false;
        Runner.CanRotate(true);
        Runner.EnableHorizontalControls();
        Runner.EnableVerticalControls();
        return base.ExitState();
    }

    public override void UpdateState()
    {
        if (dashing){
            rb2d.velocity = new Vector2(dashDirection * Runner.GetPlayerData().dashForce, 0);
        }
    }
}