using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Idle")]
public class IdleState : State<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl, attackControl;
    
    private bool canJump = false;

    private Coroutine coyoteTimer;

    public override void CaptureInput()
    {
        horizontalControl = _runner.GetHorizontalControls();
        verticalControl = _runner.GetVerticalControls();
        dashControl = _runner.GetDashControls();
        attackControl = _runner.GetAttackControls();
    }

    public override void ChangeState()
    {
        if (dashControl > 0){
            _runner.SetState(typeof(DashState));
        } 
        else if (attackControl > 0){
            _runner.SetState(typeof(GroundAttackState));
        }
        else if (horizontalControl != 0){
            _runner.SetState(typeof(WalkState));
        }
        else if (verticalControl > 0 && canJump){
            _runner.SetState(typeof(JumpState));
        }
        else if ((verticalControl < 0 && !_runner.GetGroundCheck().Check()) || (_runner.GetRigidbody2D().velocity.y < 0 && !canJump)){
            _runner.SetState(typeof(FallState));
        }
    }

    public override void ExitState()
    {
        canJump = false;
    }

    public override void FixedUpdate()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void Update()
    {
        _runner.GetRigidbody2D().velocity = new Vector2(0, _runner.GetRigidbody2D().velocity.y);
        
        CheckGround();
    }

    public void CheckGround(){
        if (_runner.GetGroundCheck().Check()){
            canJump = true;
            if (coyoteTimer != null){
                _runner.StopCoroutine(coyoteTimer);
                coyoteTimer = null;
            }
        }   
        else {
            coyoteTimer = _runner.StartCoroutine(CoyoteTimer());
        }
    }

    public IEnumerator CoyoteTimer(){
        yield return new WaitForSeconds(_runner.GetPlayerData().coyoteTime);
        canJump = false;
    }
}