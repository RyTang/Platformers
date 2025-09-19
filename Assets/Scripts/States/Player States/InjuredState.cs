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

    public IEnumerator InjuredDelay()
    {
        Runner.GetSimpleFlash().Flash(Runner.GetPlayerData().injuredDuration);
        yield return new WaitForSeconds(Runner.GetPlayerData().injuredDuration);
        injured = false;
    }

    public override void CheckStateTransition()
    {
        if (!injured)
        {
            Runner.SetMainState(typeof(NormalMainState));
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.CanRotate(true);
        return base.ExitState();
    }


}