using System.Collections;
using UnityEngine;


[CreateAssetMenu(menuName = "State/Injured")]
public class InjuredState : State<PlayerController>
{
    private bool injured;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        injured = true;
        _runner.StopCoroutine(InjuredDelay());
        _runner.StartCoroutine(InjuredDelay());
    }

    public IEnumerator InjuredDelay(){
        yield return new WaitForSeconds(_runner.GetPlayerData().injuredDuration);
        injured = false;
    }

    public override void CaptureInput()
    {
    }

    public override void ChangeState()
    {
        if (!injured) {
            _runner.SetState(typeof(IdleState));
        }
    }

    public override void ExitState()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void Update()
    {
    }
}