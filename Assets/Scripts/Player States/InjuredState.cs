using System.Collections;
using UnityEngine;


[CreateAssetMenu(menuName = "Player State/Injured")]
public class InjuredState : BaseState<PlayerController>
{
    private bool injured;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        injured = true;
        Runner.StopCoroutine(InjuredDelay());
        Runner.StartCoroutine(InjuredDelay());
    }

    public IEnumerator InjuredDelay(){
        yield return new WaitForSeconds(Runner.GetPlayerData().injuredDuration);
        injured = false;
    }

    public override void CaptureInput()
    {
    }

    public override void CheckStateTransition()
    {
        if (!injured) {
            Runner.SetMainState(typeof(NormalMainState));
        }

        // TODO: Need to change this to adjust for multiple injured moments
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