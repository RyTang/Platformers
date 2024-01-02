using UnityEngine;

[CreateAssetMenu(menuName = "State/Jump")]
public class JumpState : State<PlayerController>
{
    Rigidbody2D rb2d;

    private float horizontalControl, verticalControl, dashControl;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        rb2d = _runner.GetRigidbody2D();
        rb2d.AddForce(new Vector2(0f, _runner.GetPlayerData().jumpForce), ForceMode2D.Impulse);
        _runner.GetAnimator().SetBool(PlayerAnimation.isJumping, true);
        _runner.GetAnimator().SetTrigger(PlayerAnimation.triggerJump);
    }

    public override void CaptureInput()
    {
        horizontalControl = _runner.GetHorizontalControls();
        verticalControl = _runner.GetVerticalControls();
        dashControl = _runner.GetDashControls();
    }

    public override void ChangeState()
    {
        if (dashControl > 0){
            _runner.SetState(typeof(DashState));
        }
        else if (verticalControl <= 0 || rb2d.velocity.y <= 0){
            _runner.SetState(typeof(FallState));
        }
        else if (horizontalControl != 0 && _runner.GetWallCheck().Check()){
            _runner.SetState(typeof(WallClingState));
        }
    }

    public override void ExitState()
    {
        _runner.GetAnimator().SetBool(PlayerAnimation.isJumping, false);
    }

    public override void FixedUpdate()
    {
        if (horizontalControl == 0) {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }
        else if (horizontalControl != 0){
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