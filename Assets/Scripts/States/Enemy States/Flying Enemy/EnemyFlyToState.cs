using UnityEngine;

[CreateAssetMenu(menuName = "Enemy State/Enemy Fly To State")]
public class EnemyFlyToState : EnemyMoveToState
{
    public override void FixedUpdateState()
    {
        if (reachedDestination) return;
        
        Vector2 targetDirection = (positionToMoveTo - Runner.transform.position).normalized;
        
        Runner.GetRigidbody2D().velocity = targetDirection * Runner.GetBasicEnemydata().moveSpeed;
        // TODO: Consider if Jumping is an option;
    }
}