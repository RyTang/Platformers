using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Wall Cling")]
public class WallClingState : State<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl;
    private Rigidbody2D rb2d;

    private bool canJump = false;

    private Coroutine clingDelay;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        rb2d = parent.GetRigidbody2D();

        if (clingDelay != null){
            _runner.StopCoroutine(clingDelay);
        }

        canJump = false;

        clingDelay = _runner.StartCoroutine(ClingDelay());
    }


    public override void CaptureInput()
    {
        verticalControl = _runner.GetVerticalControls();
        horizontalControl = _runner.GetHorizontalControls();
        dashControl = _runner.GetDashControls();
    }

    public override void ChangeState()
    {
        if (dashControl > 0 && _runner.GetWallCheck().Check()){
            Vector3 localScale = _runner.transform.localScale;
            localScale.x *= -1f;
            _runner.transform.localScale = localScale;
            _runner.SetState(typeof(DashState));
        }
        
        else if (!_runner.GetWallCheck().Check() || (horizontalControl != _runner.transform.localScale.x && horizontalControl != 0)){
            _runner.SetState(typeof(FallState));
        }
        else if (_runner.GetGroundCheck().Check()){
            _runner.SetState(typeof(LandState));
        }
        else if (verticalControl > 0 & canJump){
            _runner.SetState(typeof(WallJumpState));
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

        rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Clamp(rb2d.velocity.y, -_runner.GetPlayerData().wallSlidingSpeed, _runner.GetPlayerData().wallSlidingSpeed));
    }

    public IEnumerator ClingDelay(){
        yield return new WaitForSeconds(_runner.GetPlayerData().wallJumpDuration);
        canJump = true;
    }

    public IEnumerator CoyoteTimer(){
        yield return new WaitForSeconds(_runner.GetPlayerData().coyoteTime);
        canJump = false;
    }
}
