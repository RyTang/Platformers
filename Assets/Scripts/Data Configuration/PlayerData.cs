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
    public float sprintState = 10f;
    public float jumpForce = 10f;
    public float coyoteTime = 0.3f;
    // TODO: add player Gravity Scale to affect Physics Models
    public float gravityScale = 1;
    public float mass = 1;
    [Header("Slide Information")]
    public float slideDuration = 1f;
    public float slideVelocity = 10f;

    [Header("Dash Information")]
    public float dashDuration = 0.3f;
    public float dashForce = 5;
    public float dashCooldown = 1f;

    [Header("Injured State")]

    public float injuredDuration = 0.5f;
    
    [Header("Fall Information")]
    public float horizontalFallSpeed = 5f;
    public float fallGravityMultiplier = 3.5f;
    public float terminalFallSpeed = 30;

    [Header("Free Fall Information")]
    public float horizontalFreeFallSpeed = 3f;
    public float freeFallGravityMultiplier = 5.5f;
    public float terminalFreeFallSpeed = 40;
    
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
        health = maxHealth;
    }
}