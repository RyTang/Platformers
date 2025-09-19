using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Climbing/Hanging")]
public class NormalLedgeHangState : BaseState<PlayerController>
{
    private float verticalControls, horizontalControls;
    private float initialGravity;
    private bool controlDelayedFinished, controlReleased;

    private LedgeIndicator chosenLedge;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        // Stop All Gravity
        initialGravity = Runner.GetRigidbody2D().gravityScale;
        Runner.GetRigidbody2D().gravityScale = 0;
        Runner.GetRigidbody2D().velocity = Vector2.zero;

        if (!Runner.GetLedgeCheck().Check()) {
            return;
        }

        
        Runner.GetAnimator().SetBool(PlayerAnimation.isHoldingLedgeBool, true);
        controlDelayedFinished = true;
        controlReleased = true;

        chosenLedge = GetChosenLedge().GetComponent<LedgeIndicator>();

        // Set position of hanging
        Runner.transform.position = chosenLedge.GetHangPosition();
    }

    private GameObject GetChosenLedge(){
        List<GameObject> ledges = Runner.GetLedgeCheck().GetObjectsInCheck();
        
        // Get the one that is closes to the player collision checker.
        Vector2 handPosition = Runner.GetLedgeCheck().transform.position;
        
        return ledges.OrderBy(ledge => Vector2.Distance(handPosition, ledge.transform.position)).FirstOrDefault();
    }

    public override void CaptureInput()
    {
        base.CaptureInput();
        float currentVerticalControls = Runner.GetVerticalControls();
        horizontalControls = Runner.GetHorizontalControls();
        if (currentVerticalControls < 1 || controlDelayedFinished){
            verticalControls = currentVerticalControls;
            controlReleased = true;
        }
    }

    public override void CheckStateTransition()
    {
        bool controlsNotFacingLedge = horizontalControls != 0 && horizontalControls != Mathf.Sign(Runner.transform.localScale.x);
        if (controlsNotFacingLedge || verticalControls < 0 || chosenLedge == null ) {
            CurrentSuperState.SetSubState(typeof(NormalFallState));
        }
        else if (controlReleased && verticalControls > 0) {
            CurrentSuperState.SetSubState(typeof(NormalLedgeClimbState), chosenLedge);
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isHoldingLedgeBool, false);
        Runner.GetRigidbody2D().gravityScale = initialGravity;
        yield break;
    }
}