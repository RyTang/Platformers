using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State Hierachy/GroundAttackHierachy")]
public class GroundAttackHierachy : HierarchicalState<PlayerController>
{
    private bool attacking;
    private float attackControl;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        // Prevent from Moving  
        attacking = true;

        _runner.GetRigidbody2D().velocity = new Vector2(0, _runner.GetRigidbody2D().velocity.y);

        SetSubState(typeof(GroundAttackState1));
    }

    public override void CaptureInput()
    {
        attackControl = _runner.GetAttackControls();
    }

    public override void ExitState()
    {
        attacking = false;
    }

    public override void FixedUpdate()
    {
    }

    public override void ChangeState()
    {
        if (!attacking){
            _runner.SetState(typeof(IdleState));
        }
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void Update()
    {
    }
}