using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Dash")]
public class DashState : State<PlayerController>
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
            _runner.SetState(typeof(IdleState));
            return;
        }

        rb2d = parent.GetRigidbody2D();

        if (currentDashDelay != null) {
            _runner.StopCoroutine(currentDashDelay);
            currentDashDelay = null;
        }
        
        dashing = true;
        _runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, true);
        canDash = false;
        dashDirection = Mathf.Clamp(_runner.transform.localScale.x, -1, 1);

        if (dashing){
            currentDashDelay = _runner.StartCoroutine(WallDashDelay());
        }
    }

    private IEnumerator WallDashDelay(){
        yield return new WaitForSeconds(_runner.GetPlayerData().dashDuration);
        _runner.GetAnimator().SetBool(PlayerAnimation.isDashingBool, false);
        dashing = false;
        yield return new WaitForSeconds(_runner.GetPlayerData().dashCooldown);
        canDash = true;
    }

    public override void CaptureInput()
    {
    }

    public override void ChangeState()
    {
        if (!dashing){
            _runner.SetState(typeof(WalkState));
        }
    }

    public override void ExitState()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void Update()
    {
        if (dashing){
            rb2d.velocity = new Vector2(dashDirection * _runner.GetPlayerData().dashForce, 0);
        }
    }
}