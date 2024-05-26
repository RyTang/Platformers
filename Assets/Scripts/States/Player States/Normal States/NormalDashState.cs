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
        IsRootState = false;

        if (!canDash && currentDashDelay != null){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalIdleState)));
            return;
        }

        rb2d = parent.GetRigidbody2D();

        if (currentDashDelay != null) {
            Runner.StopCoroutine(currentDashDelay);
            currentDashDelay = null;
        }
        
        dashing = true;
        Runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, true);
        canDash = false;
        dashDirection = Mathf.Clamp(Runner.transform.localScale.x, -1, 1);

        if (dashing){
            currentDashDelay = Runner.StartCoroutine(WallDashDelay());
        }
    }

    private IEnumerator WallDashDelay(){
        yield return new WaitForSeconds(Runner.GetPlayerData().dashDuration);
        Runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, false);
        dashing = false;
        yield return new WaitForSeconds(Runner.GetPlayerData().dashCooldown);
        canDash = true;
    }
    public override void CaptureInput()
    {
    }

    public override void CheckStateTransition()
    {
        if (!dashing && !Runner.GetGroundCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFallState)));
        }
        else if (!dashing && Runner.GetGroundCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalIdleState)));
        }
        else if (Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check()){
            Runner.StopCoroutine(currentDashDelay);
            currentDashDelay = null;
            Runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, false);
            canDash = true;
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalWallClingState)));
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void UpdateState()
    {
        if (dashing){
            rb2d.velocity = new Vector2(dashDirection * Runner.GetPlayerData().dashForce, 0);
        }
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