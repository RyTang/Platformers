using UnityEngine;

public static class PlayerAnimation {
    public static int jumpTrigger =  Animator.StringToHash("jumpTrigger");
    public static int isJumping = Animator.StringToHash("isJumping");
    public static int isIdleBool = Animator.StringToHash("isIdle");
    public static int isRunningBool = Animator.StringToHash("isRunning");
    public static int isFallingBool = Animator.StringToHash("isFalling");
    public static int isLandingBool = Animator.StringToHash("isLanding");
    public static int isDashingBool = Animator.StringToHash("isDashing");
    public static int isWallClingingBool = Animator.StringToHash("isWallClinging");
    public static int wallJumpTrigger = Animator.StringToHash("wallJumpTrigger");
    public static int isWallJumpingBool = Animator.StringToHash("isWallJumping");
    public static int attackTrigger = Animator.StringToHash("attackTrigger");
    public static int isAttackingBool = Animator.StringToHash("isAttacking");
}