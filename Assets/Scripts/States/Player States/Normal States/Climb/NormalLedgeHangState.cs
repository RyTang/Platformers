using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Climbing/Hanging")]
public class NormalLedgeHangState : BaseState<PlayerController>
{
    private float verticalControls, horizontalControls, jumpControl;
    private bool controlDelayedFinished, controlReleased, foundLedge;

    private LedgeIndicator chosenLedge;
    private Vector2 hangPos, standPos;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        // Stop All Gravity
        Runner.GetRigidbody2D().gravityScale = 0;
        Runner.GetRigidbody2D().velocity = Vector2.zero;

        Debug.Log("Entered Ledge Hang State");

        Runner.CanRotate(false);

        foundLedge = Runner.GetHybridLedgeDetector().TryFindLedge(out chosenLedge, out hangPos, out standPos);

        if (!foundLedge)
        {
            Debug.Log("Could not find ledge, falling");
            return;
        }


        Runner.GetAnimator().SetBool(PlayerAnimation.isHoldingLedgeBool, true);
        controlDelayedFinished = true;
        controlReleased = true;

        // Set position of hanging
        Runner.transform.position = hangPos;

        // Refreshes Movements Abilities
        Runner.RefreshAirStep();
    }

    public override void CaptureInput()
    {
        base.CaptureInput();
        jumpControl = Runner.GetJumpControl();
        float currentVerticalControls = Runner.GetVerticalControl();
        // TODO: Consider if should consider jump control here
        horizontalControls = Runner.GetHorizontalControl();
        if (currentVerticalControls < 1 || controlDelayedFinished){
            verticalControls = currentVerticalControls;
            controlReleased = true;
        }
    }

    public override void CheckStateTransition()
    {
        bool controlsNotFacingLedge = horizontalControls != 0 && horizontalControls != Mathf.Sign(Runner.IsFacingRight() ? 1 : -1);
        if (controlsNotFacingLedge || verticalControls < 0 || !foundLedge)
        {
            Debug.Log("Letting go of ledge");
            CurrentSuperState.SetSubState(typeof(NormalFallState));
        }
        else if (controlReleased)
        {
            if (verticalControls > 0)
            {
                Debug.Log("Climbing up ledge");
                CurrentSuperState.SetSubState(typeof(NormalLedgeClimbState), chosenLedge);
            }
            else if (jumpControl > 0)
            {
                // TODO: Need to create a new ledge jump state
                CurrentSuperState.SetSubState(typeof(NormalJumpState));
            }
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isHoldingLedgeBool, false);
        Runner.GetRigidbody2D().gravityScale = Runner.GetPlayerData().gravityScale;
        Runner.CanRotate(true);
        yield break;
    }
}