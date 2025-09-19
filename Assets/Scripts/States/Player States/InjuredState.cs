using System.Collections;
using UnityEngine;


[CreateAssetMenu(menuName = "Player State/Injured")]
public class InjuredState : BaseState<PlayerController>
{
    private bool injured;

    public override void EnterState(PlayerController parent)
    {
        Debug.Log("Injured");
        base.EnterState(parent);
        // TODO: Add the simple flash animation thingy
        injured = true;
        Runner.StopCoroutine(InjuredDelay());
        Runner.StartCoroutine(InjuredDelay());
    }

    public IEnumerator InjuredDelay()
    {
        yield return new WaitForSeconds(Runner.GetPlayerData().injuredDuration);
        injured = false;
    }

    public override void CaptureInput()
    {
    }

    public override void CheckStateTransition()
    {
        if (!injured)
        {
            Runner.SetMainState(typeof(NormalMainState));
        }

        // TODO: Need to change this to adjust for multiple injured states
    }

    public override IEnumerator ExitState()
    {
        Runner.SetPlayerSpriteDirectionMutable(true);
        return base.ExitState();
    }


}