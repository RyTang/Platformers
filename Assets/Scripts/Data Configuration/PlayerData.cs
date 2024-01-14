using UnityEngine;

[CreateAssetMenu(menuName = "Configs/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Basic Information")]
    public int health = 10; 
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float coyoteTime = 0.3f;

    [Header("Dash Information")]
    public float dashDuration = 0.3f;
    public float dashForce = 5;
    public float dashCooldown = 1f;

    [Header("Sprint State")]
    public float maxEnergyBar = 100f;
    public float currentEnergy = 100f;
    public float maxSprintSpeed = 10f;
    public float sprintDepletionRate = 10f;
    public float energyRecoveryRate = 5f;
    public float accelerationTime = 2.0f;
    public float decelerationTime = 1.0f;

    [Header("Injured State")]

    public float injuredDuration = 0.5f;
    
    [Header("Fall Information")]
    public float horizontalFallSpeed = 5f;
    public float fallGravityMultiplier = 3.5f;
    public float terminalFallSpeed = 30;
    
    [Header("Land Information")]
    public float landDelay = 0.5f;
    public float landVelocityThreshold = 20f;

    [Header("Wall Jump Information")]
    public float wallSlidingSpeed = 2f;
    public Vector2 wallJumpForce = new Vector2(10, 5);
    public float wallJumpDuration = 0.5f;
    public float clingDelay = 0.15f;
    
    [Header("Attack Information")]
    public float attackTime;
    public int attackDamage;
}