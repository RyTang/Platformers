using UnityEngine;

[CreateAssetMenu(menuName = "State/Fall")]
public class FallState : State<PlayerController>
{
    private float horizontalControl, dashControl;

    private float initialLocalGravity;

    Rigidbody2D rb2d;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        rb2d = parent.GetRigidbody2D();
        initialLocalGravity = rb2d.gravityScale;


        rb2d.gravityScale = initialLocalGravity * _runner.GetPlayerData().fallGravityMultiplier;

        // TODO: Look into doing coyote Timing here. Need to figure out how to differentiate Wall Jump vs Jump
    }

    public override void CaptureInput()
    {
        horizontalControl = _runner.GetHorizontalControls();
        dashControl = _runner.GetDashControls();
    }

    public override void ChangeState()
    {   
        if (dashControl > 0){
            _runner.SetState(typeof(DashState));
        }
       
        else if (_runner.GetGroundCheck().Check()){
            _runner.SetState(typeof(LandState));
        }
        else if (horizontalControl != 0 && _runner.GetWallCheck().Check()){
            _runner.SetState(typeof(WallClingState));
        }
    }

    public override void ExitState()
    {
        rb2d.gravityScale = initialLocalGravity;
    }

    public override void FixedUpdate()
    {

    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
        
    }

    public override void Update()
    {
        float x_velocity = 0;

        if (horizontalControl != 0){
            x_velocity = horizontalControl > 0 ? _runner.GetPlayerData().horizontalFallSpeed : -_runner.GetPlayerData().horizontalFallSpeed;
        }
        
        rb2d.velocity = new Vector2(x_velocity, Mathf.Clamp(rb2d.velocity.y, -_runner.GetPlayerData().terminalFallSpeed, float.MaxValue));
    }
}