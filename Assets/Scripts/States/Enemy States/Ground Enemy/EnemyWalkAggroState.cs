using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy State/Enemy Chase")]
public class EnemyWalkAggroState : EnemyAggroState
{
    public override void CaptureInput()
    {
    }
    public override void FixedUpdateState()
    {

        Vector2 targetDirection = (objToChase.transform.position - Runner.transform.position).normalized;
        float xDirection = targetDirection.x > 0 ? 1 : -1;
        

        Runner.GetRigidbody2D().velocity = new Vector2(xDirection * Runner.GetBasicEnemydata().moveSpeed, Runner.GetRigidbody2D().velocity.y);

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