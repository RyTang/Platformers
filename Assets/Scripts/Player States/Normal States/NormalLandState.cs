using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Land")]
public class NormalLandState : BaseState<PlayerController>
{
    private bool canMove;
    private float landVelocity;

    public override void EnterState(PlayerController parent, object objToPass)
    {
        landVelocity = Mathf.Abs((float) objToPass);
        base.EnterState(parent, objToPass);
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
            CurrentSuperState.SetSubState(Runner.GetState(typeof(NormalIdleState)));
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