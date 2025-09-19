using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Enemy Data")]
public class BasicEnemyData : ScriptableObject
{
    [Header("Basic Information")]
    public int health = 10;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Vector3 startingPosition;

    [Header("Attack Information")]
    public float attackTime;
    public int attackDamage;
    public float attackCooldown;
    public float chaseDuration;
    public float knockbackForce = 1f;

    // TODO: Add Stagger Duration and etc
    
}