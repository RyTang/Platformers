using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Climbing/Climb")]
public class NormalLedgeClimbState : BaseState<PlayerController>
{
    private bool assignedLedge = false; 
    private bool doneClimbing;
    private Vector2 hangPos, standPos;
    
    public override void EnterState(PlayerController parent, object objToPass)
    {
        // Retrieve object
        if (objToPass is LedgeIndicator indicator && indicator != null)
        {
            Debug.Log("Has been given ledge indicator");
            assignedLedge = true;
            hangPos = indicator.GetHangPosition();
            standPos = indicator.GetStandPosition();
        }
        base.EnterState(parent, objToPass);
    }


    public override void EnterState(PlayerController parent)
    {
        Debug.Log("Entered Ledge Climb State");
        base.EnterState(parent);

        Runner.GetRigidbody2D().gravityScale = 0;
        Runner.GetRigidbody2D().velocity = Vector2.zero;
        Runner.CanRotate(false);

        doneClimbing = false;

        // If wasn't given a ledge Object
        if (!assignedLedge) {
            assignedLedge = Runner.GetHybridLedgeDetector().TryFindLedge(out _, out hangPos, out standPos);
            Debug.Log($"Found LEdge with positions {hangPos} and {standPos}");
        }

        if (assignedLedge) {
            parent.AnimationEvent += OnAnimationEventTriggered;
            Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerLedgeClimb);
            // Set position of hanging
            Runner.transform.position = hangPos;
        }
    }

    private void OnAnimationEventTriggered(AnimationEventTrigger eventTrigger){
        if (eventTrigger == AnimationEventTrigger.FINISH_LEDGE_CLIMB) {
            doneClimbing = true;
        }
    }


    public override void CheckStateTransition()
    {
        if (doneClimbing) {
            Runner.GetAnimator().SetBool(PlayerAnimation.isIdleBool, true);
            Runner.transform.position = standPos;
            CurrentSuperState.SetSubState(typeof(NormalIdleState));
        }
        else if (!assignedLedge){
            CurrentSuperState.SetSubState(typeof(NormalFallState));
        }
    }

    public override IEnumerator ExitState()
    {
        // Reset Gravity and motion
        Runner.CanRotate(true);
        Runner.GetRigidbody2D().gravityScale = Runner.GetPlayerData().gravityScale;
        assignedLedge = false;
        yield break;
    }
}