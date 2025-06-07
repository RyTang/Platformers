using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Climbing/Climb")]
public class NormalLedgeClimbState : BaseState<PlayerController>
{
    private bool doneClimbing;
    private LedgeIndicator chosenLedge;
    
    private float initialGravity;

    public override void EnterState(PlayerController parent, object objToPass)
    {
        Debug.Log("Entered Ledge Climb State");
        base.EnterState(parent, objToPass);
        
        // Retrieve object
        if (objToPass is LedgeIndicator){
            chosenLedge = (LedgeIndicator) objToPass;
        }
    }


    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        initialGravity = Runner.GetRigidbody2D().gravityScale;
        Runner.GetRigidbody2D().gravityScale = 0;
        Runner.GetRigidbody2D().velocity = Vector2.zero;

        doneClimbing = false;

        // TODO: Get hanging position and then get the standing up position
        // If wasn't given a ledge Object
        if (chosenLedge == null && Runner.GetLedgeCheck().Check()) {
            chosenLedge = GetChosenLedge().GetComponent<LedgeIndicator>();
        }

        if (chosenLedge != null) {
            // parent.AnimationEvent += OnAnimationEventTriggered;
            Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerLedgeClimb);
            // Set position of hanging
            Runner.transform.position = chosenLedge.GetHangPosition();
            Runner.StopCoroutine(StartLedgeClimb()); // TODO: Once animation is completed, shift this to use the Animation Event Handler
            Runner.StartCoroutine(StartLedgeClimb());
        }
    }

    private GameObject GetChosenLedge(){
        List<GameObject> ledges = Runner.GetLedgeCheck().GetObjectsInCheck();
        
        // Get the one that is closes to the player collision checker.
        Vector2 handPosition = Runner.GetLedgeCheck().transform.position;
        
        return ledges.OrderBy(ledge => Vector2.Distance(handPosition, ledge.transform.position)).FirstOrDefault();
    }

    private void OnAnimationEventTriggered(AnimationEventTrigger eventTrigger){
        if (eventTrigger == AnimationEventTrigger.FINISH_LEDGE_CLIMB) {
            doneClimbing = true;
        }
    }


    private IEnumerator StartLedgeClimb(){
        yield return new WaitForSeconds(Runner.GetPlayerData().climbDuration); // TODO: Add Ledge Climb Delay

        doneClimbing = true;
    }

    public override void CheckStateTransition()
    {
        if (doneClimbing) {
            Runner.GetAnimator().SetBool(PlayerAnimation.isIdleBool, true);
            Runner.transform.position = chosenLedge.GetStandPosition();
            CurrentSuperState.SetSubState(typeof(NormalIdleState));
        }
        else if (chosenLedge == null){
            CurrentSuperState.SetSubState(typeof(NormalFallState));
        }
    }

    public override IEnumerator ExitState()
    {   
        // Reset Gravity and motion
        Runner.GetRigidbody2D().gravityScale = initialGravity;
        yield break;
    }
}