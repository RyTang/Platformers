using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Land")]
public class LandState : BaseState<PlayerController>
{

    private bool canMove;

    public override void EnterState(PlayerController parent, float floatVariable)
    {
        base.EnterState(parent, floatVariable);

        canMove = !(floatVariable >= Runner.GetPlayerData().landVelocityThreshold);
        LandCheck(canMove);
    }

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        // FIXME: Velocity is 0 due to when entering the state, it has already touched the ground

        Debug.Log("Landing Velocity: " + Runner.GetRigidbody2D().velocity.y);

        canMove = !(Mathf.Abs(Runner.GetRigidbody2D().velocity.y) >= Runner.GetPlayerData().landVelocityThreshold);
        LandCheck(canMove);
    }

    private void LandCheck(bool canMove)
    {
        if (!canMove)
        {
            // TODO: Create Animation here
            Runner.StopCoroutine(LandDelay());
            Runner.StartCoroutine(LandDelay());
        }

        Runner.GetAnimator().SetBool(PlayerAnimation.isLandingBool, true);
    }

    private IEnumerator LandDelay(){
        yield return new WaitForSeconds(Runner.GetPlayerData().landDelay);
        canMove = true;
    }

    public override void CaptureInput()
    {
    }
 
    public override void CheckStateTransition()
    {
        if (canMove) {
            Runner.SetMainState(typeof(IdleState));
        }
    }

    public override void ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isLandingBool, false);
    }

    public override void FixedUpdateState()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void UpdateState()
    {
    }


    public override void InitialiseSubState()
    {
    }
}