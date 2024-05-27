using UnityEngine;

[CreateAssetMenu(menuName = "Enemy State/Enemy Walk To State")]
public class EnemyWalkToState : EnemyMoveToState
{
    public override void FixedUpdateState()
    {
        if (reachedDestination) return;
        
        Vector2 targetDirection = (positionToMoveTo - Runner.transform.position).normalized;
        float xDirection = targetDirection.x > 0 ? 1 : -1;
        

        Runner.GetRigidbody2D().velocity = new Vector2(xDirection * Runner.GetBasicEnemydata().moveSpeed, Runner.GetRigidbody2D().velocity.y);

        // TODO: Consider if Jumping is an option;
    }
}