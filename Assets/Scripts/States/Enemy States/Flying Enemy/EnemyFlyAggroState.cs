using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy State/Enemy Fly Chase")]
public class EnemyFlyAggroState : EnemyAggroState
{
    public override void CaptureInput()
    {
    }

    public override void FixedUpdateState()
    {
        Vector2 targetDirection = (objToChase.transform.position - Runner.transform.position).normalized;
        
        Runner.GetRigidbody2D().velocity = targetDirection * Runner.GetBasicEnemydata().moveSpeed;
        // TODO: Consider if Jumping is an option;
    }

    public override void InitialiseSubState()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void OnStateCollisionStay(Collision2D collision)
    {
    }

    public override void OnStateCollisionExit(Collision2D collision)
    {
    }
}