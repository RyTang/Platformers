using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(menuName = "Configs/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Basic Information")]
    public int maxHealth = 10;
    public int health = 10; 
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float coyoteTime = 0.3f;
    // TODO: add player Gravity Scale to affect Physics Models
    public float gravityScale = 1;
    public float mass = 1;

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
    public float accelerationSpeed = 2.0f;
    public float sprintJumpMultiplier = 2.0f;
    public float sprintWallJumpMultiplier = 2f;
    public float sprintJumpDuration = 1.5f;
    public float sprintJumpHeight = 6f;
    public float maxSprintJumpVelocity = 5;
    public float minSprintXFallSpeed = 3;
    public float sprintWallSlowdown = 3.0f;

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
    public float knockbackForce = 1f;


    // Player Inate Data
    private float maxJumpHeight;

    private float maxJumpDistance;

    public float GetMaxJumpHeight(){
        float g = gravityScale * Physics2D.gravity.magnitude;
        float v0 = jumpForce / mass;
        maxJumpHeight = (v0 * v0)/(2*g);

        return maxJumpHeight;
    }

    public float GetMaxJumpDistance(){
        // Require Max Jump Height to calculate maximum distance
        if (maxJumpHeight == 0) GetMaxJumpHeight();

        // Need to bear in mind of Fall Velocity Increasing Speed
        float jumpGravity = gravityScale * Physics2D.gravity.magnitude;        
        float timeToPeak = jumpForce / jumpGravity;


        float fallGravity = fallGravityMultiplier * Physics2D.gravity.magnitude;
        
        float fallTime = Mathf.Sqrt((2 * maxJumpHeight) / fallGravity);

        maxJumpDistance = (timeToPeak + fallTime) * moveSpeed;

        return maxJumpDistance;
    }

    public void ResetPlayerStats(){
        currentEnergy = maxEnergyBar;
        health = maxHealth;
    }
}