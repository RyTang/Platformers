using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Dash")]
public class NormalDashState : BaseState<PlayerController>
{
    private Rigidbody2D rb2d;

    private Coroutine currentDashDelay;

    private bool canDash = true;
    private bool dashing;
    private float dashDirection;
    

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        if (!canDash && currentDashDelay != null){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalIdleState)));
            return;
        }

        rb2d = parent.GetRigidbody2D();
        // Disable to prevent Direction from affecting velocity in this case -> If dashing then should not control so easily
        Runner.DisableHorizontalControls();
        Runner.DisableVerticalControls();

        if (currentDashDelay != null) {
            Runner.StopCoroutine(currentDashDelay);
            currentDashDelay = null;
        }
        
        dashing = true;
        Runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, true);
        dashDirection = Mathf.Clamp(Runner.transform.localScale.x, -1, 1);
        canDash = false;
        currentDashDelay = Runner.StartCoroutine(WallDashDelay());
    }

    private IEnumerator WallDashDelay(){
        yield return new WaitForSeconds(Runner.GetPlayerData().dashDuration);
        Runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, false);
        dashing = false;
        yield return new WaitForSeconds(Runner.GetPlayerData().dashCooldown);
        canDash = true;
    }

    public override void CheckStateTransition()
    {
        if (!dashing && !Runner.GetGroundCheck().Check()){
            CurrentSuperState.SetSubState(typeof(NormalFallState));
        }
        else if (!dashing && Runner.GetGroundCheck().Check()){
            CurrentSuperState.SetSubState(typeof(NormalIdleState));
        }
        else if (Runner.GetLedgeCheck().Check()) {
            CurrentSuperState.SetSubState(typeof(NormalLedgeHangState));
        }
        else if (Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check()){
            // FIXME: When holding direction in wall, it will trigger touching wall as the animation changes direction for a split second
            // TODO: Add Short Timer to prevent retouching
            Debug.Log("Touching Wall");
            Runner.StopCoroutine(currentDashDelay);
            currentDashDelay = null;
            Runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, false);
            canDash = true;
            CurrentSuperState.SetSubState(typeof(NormalWallClingState));
        }
    }

    public override IEnumerator ExitState()
    {
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