using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Player State/Speed State/Land State")]
public class SpeedLandState : BaseState<PlayerController>
{
    Rigidbody2D rb2d;
    float horizontalControl;
    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        // TODO: Enters State while slowing down -> Carries forward momentum
        
        rb2d = Runner.GetRigidbody2D();
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
    }
 
    public override void CheckStateTransition()
    {  
        if (horizontalControl != 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SpeedRunState)));    
        }
        else if (rb2d.velocity.x == 0){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(SpeedIdleState)));
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

    public override void OnStateCollisionStay(Collision2D collision)
    {
    }

    public override void OnStateCollisionExit(Collision2D collision)
    {
    }
}