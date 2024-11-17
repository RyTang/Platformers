using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Sprint State/Dash")]
public class SprintDashState : BaseState<PlayerController>
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
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SprintIdleState)));
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
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SprintFallState)));
        }
        else if (!dashing && Runner.GetGroundCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SprintIdleState)));
        }
        else if (Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check()){
            // FIXME: When holding direction in wall, it will trigger touching wall as the animation changes direction for a split second
            // TODO: Add Short Timer to prevent retouching
            Debug.Log("Touching Wall");
            Runner.StopCoroutine(currentDashDelay);
            currentDashDelay = null;
            Runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, false);
            canDash = true;
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SprintWallClingState)));
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