using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Idle")]
public class NormalIdleState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, dashControl, attackControl, sprintControl;
    
    private bool canJump = false;

    private Coroutine coyoteTimer;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        Runner.GetAnimator().SetBool(PlayerAnimation.isIdleBool, true);
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        dashControl = Runner.GetDashControls();
        attackControl = Runner.GetAttackControls();
        sprintControl = Runner.GetSprintControls();
    }

    public override void CheckStateTransition()
    {
        if (dashControl > 0){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(NormalDashState)));
        } 
        else if (attackControl > 0){
            Debug.Log("Attacking");
            CurrentSuperState.SetSubState(Runner.GetState(typeof(GroundSubAttackOne)));
        }
        else if (horizontalControl != 0){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(NormalRunState)));
        }
        else if (verticalControl > 0 && canJump){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(NormalJumpState)));
        }
        else if ((verticalControl < 0 && !Runner.GetGroundCheck().Check()) || (Runner.GetRigidbody2D().velocity.y < 0 && !canJump)){
            CurrentSuperState.SetSubState(Runner.GetState(typeof(NormalFallState)));
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isIdleBool, false);
        canJump = false;
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
        Runner.GetRigidbody2D().velocity = new Vector2(0, Runner.GetRigidbody2D().velocity.y);
        
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