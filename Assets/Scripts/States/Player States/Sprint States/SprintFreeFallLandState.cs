using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Sprint State/Free Fall Land")]
public class SprintFreeFallLandState : BaseState<PlayerController>
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

        Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerFreeFallLanding);
    }

    private IEnumerator LandDelay(){
        yield return new WaitForSeconds(Runner.GetPlayerData().landDelay);
        canMove = true;
    }
 
    public override void CheckStateTransition()
    {
        if (canMove) {
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SprintIdleState)));
        }
    }

}
