using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "State/GroundAttackState/Root State")]
public class GroundAttackState : BaseState<PlayerController>
{
    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        InitialiseSubState();
    }

    public override void CaptureInput()
    {
    }

    public override void CheckStateTransition()
    {
        
    }


    public override void FixedUpdateState()
    {
    }

    public override void InitialiseSubState()
    {
        SetSubState(Runner.GetState(typeof(GroundSubAttackOne)));
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void UpdateState()
    {
    }

    public override void ExitState()
    {
    }
}