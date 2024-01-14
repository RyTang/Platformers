using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Land")]
public class LandState : BaseState<PlayerController>
{
    private bool canMove;
    private float landVelocity;

    public override void EnterState(PlayerController parent, float floatVariable)
    {
        landVelocity = Mathf.Abs(floatVariable);
        base.EnterState(parent, floatVariable);
    }

    public override void EnterState(PlayerController parent)
    {
        // FIXME: Double Calling Enter State when entering
        base.EnterState(parent);
        // FIXME: Velocity is 0 due to when entering the state, it has already touched the ground

        landVelocity = landVelocity != 0 ? landVelocity : Mathf.Abs(Runner.GetRigidbody2D().velocity.y);
        canMove = !(landVelocity >= Runner.GetPlayerData().landVelocityThreshold);
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

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isLandingBool, false);
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
    }


    public override void InitialiseSubState()
    {
    }
}