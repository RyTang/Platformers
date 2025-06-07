using UnityEngine;

public static class PlayerAnimation {
    public static int triggerJump =  Animator.StringToHash("triggerJump");
    public static int isJumping = Animator.StringToHash("isJumping");
    public static int isIdleBool = Animator.StringToHash("isIdle");
    public static int isRunningBool = Animator.StringToHash("isRunning");
    public static int isSprintingBool = Animator.StringToHash("isSprinting");
    public static int isFallingBool = Animator.StringToHash("isFalling");
    public static int isLandingBool = Animator.StringToHash("isLanding");
    public static int isDashingBool = Animator.StringToHash("isDashing");
    public static int isWallClingingBool = Animator.StringToHash("isWallClinging");
    public static int triggerWallJump = Animator.StringToHash("triggerWallJump");
    public static int isHoldingLedgeBool = Animator.StringToHash("isHoldingLedge");
    public static int triggerLedgeClimb = Animator.StringToHash("triggerLedgeClimb");
    public static int triggerSlide = Animator.StringToHash("triggerSlide");
    public static int isWallJumpingBool = Animator.StringToHash("isWallJumping");
    public static int triggerGroundAttack01 = Animator.StringToHash("triggerGroundAttack01");
    public static int triggerGroundAttack02 = Animator.StringToHash("triggerGroundAttack02");
    public static int triggerGroundAttack03 = Animator.StringToHash("triggerGroundAttack03");
    public static int isAttackingBool = Animator.StringToHash("isAttacking");
    public static int canAttackBool = Animator.StringToHash("canAttack");
    public static int triggerFreefall = Animator.StringToHash("triggerFreefall");
    public static int isFreeFallingBool = Animator.StringToHash("isFreeFalling");
    public static int isSlidingBool = Animator.StringToHash("isSliding");
    public static int triggerFreeFallLanding = Animator.StringToHash("triggerFreeFallLanding");
}