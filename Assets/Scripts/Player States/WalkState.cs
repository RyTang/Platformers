using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Walk")]
public class WalkState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl, attackControl;

    private Rigidbody2D rb2d;

    private bool canJump = false;
    private Coroutine coyoteTimer;


    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        rb2d = parent.GetRigidbody2D();
        canJump = false;
        Runner.GetAnimator().SetBool(PlayerAnimation.isRunningBool, true);
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        dashControl = Runner.GetDashControls();
        attackControl = Runner.GetAttackControls();
    }

    public override void CheckStateTransition()
    {
        if (dashControl > 0){
            Runner.SetMainState(typeof(DashState));
        }
        else if (attackControl > 0) {
            Runner.SetMainState(typeof(GroundSubAttackOne));
        }
        else if (horizontalControl == 0){
            Runner.SetMainState(typeof(IdleState));
        }
        else if (verticalControl > 0 && canJump) {
            Runner.SetMainState(typeof(JumpState));
        }
        else if ((verticalControl < 0 && !Runner.GetGroundCheck().Check()) || (Runner.GetRigidbody2D().velocity.y < 0 && !canJump)){
            Runner.SetMainState(typeof(FallState));
        }
        else if (horizontalControl != 0 && Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check()){
            Runner.SetMainState(typeof(WallClingState));
        }
    }
    
    public override IEnumerator ExitState(){
        canJump = false;
        Runner.GetAnimator().SetBool(PlayerAnimation.isRunningBool, false);
        yield break;
    }

    public override void FixedUpdateState()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
        
    }

    public override void UpdateState()
    {
        if (horizontalControl != 0){
            rb2d.velocity = horizontalControl > 0 ? new Vector2(Runner.GetPlayerData().moveSpeed, rb2d.velocity.y) : new Vector2(-Runner.GetPlayerData().moveSpeed, rb2d.velocity.y);
        }
        
        CheckGround();
    }

    public void CheckGround(){
        if (Runner.GetGroundCheck().Check()){
            canJump = true;
            if (coyoteTimer != null){
                Runner.StopCoroutine(coyoteTimer);
                coyoteTimer = null;
            }
        }   
        else {
            coyoteTimer = Runner.StartCoroutine(CoyoteTimer());
        }
    }

    public IEnumerator CoyoteTimer(){
        yield return new WaitForSeconds(Runner.GetPlayerData().coyoteTime);
        canJump = false;
    }


    public override void InitialiseSubState()
    {
    }
}