using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Climbing/Hanging")]
public class NormalLedgeHangState : BaseState<PlayerController>
{
    private float verticalControls;
    private float initialGravity;
    private Vector2 hangPosition, standPosition;

    private LedgeIndicator chosenLedge;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        // Stop All Gravity
        initialGravity = Runner.GetRigidbody2D().gravityScale;
        Runner.GetRigidbody2D().gravityScale = 0;
        // TODO: Play Hang Animation

        if (!Runner.GetLedgeCheck().Check()) {
            return;
        }

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
        verticalControls = Runner.GetVerticalControls();
    }

    public override void CheckStateTransition()
    {
        if (verticalControls < 0 || chosenLedge != null ) {
            CurrentSuperState.SetSubState(typeof(NormalIdleState));
        }
        else if (verticalControls > 0) {
            CurrentSuperState.SetSubState(typeof(NormalLedgeClimbState), chosenLedge);
        }
    }

    public override IEnumerator ExitState()
    {
        // TODO: End Hang Animation State
        // TODO: This might cause a weird interaction, think about how to handle gravity interactions
        Runner.GetRigidbody2D().gravityScale = initialGravity;
        yield break;
    }
}