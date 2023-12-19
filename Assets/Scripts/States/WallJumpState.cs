using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Wall Jump")]
public class WallJumpState : State<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl;
    private Rigidbody2D rb2d;
    private bool canMove;
    private Coroutine currentWallDelay;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        rb2d = parent.GetRigidbody2D();

        if (currentWallDelay != null){
            _runner.StopCoroutine(currentWallDelay);
            currentWallDelay = null;
        }
        canMove = false;
        float wallJumpDirection = -_runner.transform.localScale.x;
        rb2d.AddForce(new Vector2(wallJumpDirection * _runner.GetPlayerData().wallJumpForce.x, _runner.GetPlayerData().wallJumpForce.y), ForceMode2D.Impulse);

        _runner.GetAnimator().SetTrigger(PlayerAnimation.wallJumpTrigger);
        _runner.GetAnimator().SetBool(PlayerAnimation.isWallJumpingBool, true);

        if (_runner.transform.localScale.x * wallJumpDirection <= 0){
            Vector3 localScale = _runner.transform.localScale;
            localScale.x *= -1f;
            _runner.transform.localScale = localScale;
        }

        if (!canMove){
            currentWallDelay = _runner.StartCoroutine(WallJumpDelay());
        }
    }

    private IEnumerator WallJumpDelay(){
        yield return new WaitForSeconds(_runner.GetPlayerData().wallJumpDuration);
        canMove = true;
    }

    public override void CaptureInput()
    {   
        verticalControl = _runner.GetVerticalControls();
        horizontalControl = _runner.GetHorizontalControls();
        dashControl = _runner.GetDashControls();
    }

    public override void ChangeState()
    {

        // FIXME: If Crashing into top wall, velocity will drop to 0 hence becomes falling state
        if (canMove){
            if (dashControl > 0){
                _runner.SetState(typeof(DashState));
            }
            else if (verticalControl <= 0 || rb2d.velocity.y <= 0){
                _runner.SetState(typeof(FallState));
            }
        }
        else if (_runner.GetWallCheck().Check()){
            _runner.SetState(typeof(WallClingState));
        }
    }

    public override void ExitState()
    {
        _runner.GetAnimator().SetBool(PlayerAnimation.isWallJumpingBool, false);
    }

    public override void FixedUpdate()
    {
        if (canMove && horizontalControl != 0){
            rb2d.velocity = horizontalControl > 0 ? new Vector2(_runner.GetPlayerData().moveSpeed, rb2d.velocity.y) : new Vector2(-_runner.GetPlayerData().moveSpeed, rb2d.velocity.y);
        }
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void Update()
    {

    }
}
