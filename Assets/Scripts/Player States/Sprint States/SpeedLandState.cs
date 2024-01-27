using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Player State/Speed State/Land State")]
public class SpeedLandState : BaseState<PlayerController>
{

    public override void EnterState(PlayerController parent, object objToPass)
    {
        base.EnterState(parent, objToPass);
    }

    public override void EnterState(PlayerController parent)
    {
        // FIXME: Double Calling Enter State when entering
        base.EnterState(parent);

    }

    public override void CaptureInput()
    {
    }
 
    public override void CheckStateTransition()
    {
        CurrentSuperState.SetSubState(Runner.GetState(typeof(SpeedIdleState)));
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

    public override void OnStateCollisionStay(Collision2D collision)
    {
    }

    public override void OnStateCollisionExit(Collision2D collision)
    {
    }
}