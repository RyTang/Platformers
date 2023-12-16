// using System.Runtime.CompilerServices;
// using System;
// using UnityEngine;
// using Unity.VisualScripting;
// public class OLDPlayerController : IBasicMovement, IWallMovement
// {   
//     enum PlayerState {
//         Idle,
//         Normal,
//         Jumping,
//         Falling,
//         Landing,
//         WallSliding,
//         WallJumping,
//     }

//     private PlayerState currentState = PlayerState.Idle;


//     [SerializeField] private float jumpForce = 10f;
//     [SerializeField] private float moveSpeed = 4f;
//     [SerializeField] private float terminalFallSpeed = 10f;

//     private bool isWallSliding;
//     [SerializeField] private float wallSlidingSpeed = 2f;


//     private bool canWallJump;
//     private bool clingingWall;
//     private bool isWallJumping;
//     [SerializeField] private Vector2 wallJumpForce = new Vector2(10, 5);

//     [SerializeField] private float wallJumpDuration = 0.3f;

//     private bool canJump = true;
//     private bool touchingGround;

//     private bool isJumping = false;
//     private bool canMove = true;

//     private float localGravity;
//     [SerializeField] private float fallGravityMultiplier = 2f; 

//     private float horizontalControl, verticalControl;


//     protected override void Start()
//     {
//         base.Start();
//         InitializeVariables();
//     }

//     private void InitializeVariables()
//     {
//         canJump = true;
//         canMove = true;
//         isJumping = false;
//         clingingWall = false;
//         touchingGround = false;
//         isWallJumping = false;
//         canWallJump = false;

//         localGravity = rb2d.gravityScale;
//     }


//     protected override void Update()
//     {
//         base.Update();
        
//         // Get Control Inputs
//         GetControlInputs();

//         switch (currentState) {
//             case PlayerState.Idle:
//                 HandleIdleState();
//                 break;
//             case PlayerState.Normal:
//                 HandleNormalState();
//                 break;
//             case PlayerState.Jumping:
//                 HandleJumpingState();
//                 break;
//             case PlayerState.Falling:
//                 HandleFallingState();
//                 break;
//             case PlayerState.Landing:
//                 HandleLandingState();
//                 break;            
//             case PlayerState.WallSliding:
//                 HandleWallSlidingState();
//                 break;
//             case PlayerState.WallJumping:
//                 HandleWallJumpingState();
//                 break;
            
//         }
//     }

//     private void HandleIdleState(){
//         NormalMovementControls();
//         if (verticalControl > 0 && canJump && !isJumping)
//         {
//             Jump();
//         }

//         // Control Gravity
//         HandleGravity();
//     }

//     private void HandleNormalState()
//     {
//         NormalMovementControls();
//         if (verticalControl > 0 && canJump && !isJumping)
//         {
//             Jump();
//         }

//         // Control Gravity
//         HandleGravity();
//     }

    

//     private void HandleJumpingState(){
//         NormalMovementControls();
//         HandleGravity();
//     }

//     private void HandleFallingState(){
//         NormalMovementControls();
//         HandleGravity();
//     }

//     private void HandleLandingState(){
//         // TODO: Add Delay if landing at terminal Velocity
//         NormalMovementControls();
//         HandleGravity();
//     }

//     private void HandleWallSlidingState(){
//         WallSlidingMovementControls();
//         HandleGravity();
//         WallSlide();

//         if (verticalControl > 0 && canWallJump && !isJumping)
//         {
//             WallJump();
//         }
//     }

//     private void HandleWallJumpingState(){
//         HandleGravity();
//         if (!isWallJumping){
//             NormalMovementControls();
//         }
//     }


//     private void HandleGravity()
//     {
//         if ((rb2d.velocity.y < 0) || (rb2d.velocity.y > 0 && isJumping && verticalControl <= 0))
//         {
//             if (!isWallJumping){ 
//                 ChangePlayerState(PlayerState.Falling);
//             }
//             rb2d.gravityScale = localGravity * fallGravityMultiplier;
//             rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Clamp(rb2d.velocity.y, -terminalFallSpeed, float.MaxValue));
//         }
//         else if (rb2d.velocity.y == 0)
//         {
//             rb2d.gravityScale = localGravity;
//         }
//     }

//     private void GetControlInputs(){
//         horizontalControl = Input.GetAxisRaw("Horizontal");
//         verticalControl = Input.GetAxisRaw("Vertical");
//     }

//     private void NormalMovementControls(){
//         if (horizontalControl != 0 && !isWallSliding){
//             if (canMove) Move(new Vector2(horizontalControl, 0f));
//         }
//         else if (horizontalControl == 0){
//             if (verticalControl == 0 && rb2d.velocity.y == 0) ChangePlayerState(PlayerState.Idle);
            
//             rb2d.velocity = new Vector2(0, rb2d.velocity.y);
//         }   
//     }

//     private void WallSlidingMovementControls(){
//         if (isWallSliding && horizontalControl != Mathf.Clamp(transform.localScale.x, -1, 1)){
//             if (canMove) Move(new Vector2(horizontalControl, 0f));
//         }
//     }

//     public void Jump()
//     {
//         if (!canJump && isJumping) return;

//         ChangePlayerState(PlayerState.Jumping);
//         canJump = false;
//         isJumping = true;
//         rb2d.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
//     }

//     public void Move(Vector2 direction)
//     {

//         ChangePlayerState(PlayerState.Normal);
//         rb2d.velocity = direction.x > 0 ? new Vector2(moveSpeed, rb2d.velocity.y) : new Vector2(-moveSpeed, rb2d.velocity.y);
//     }

//     public void TouchingGround(bool touching){
//         if (touching && !touchingGround){
//             ChangePlayerState(PlayerState.Landing);
//             canJump = true;
//             touchingGround = true;
//             isJumping = false;
//             isWallJumping = false;            
//         }
//         else if (!touching){
//             touchingGround = false;
//             canJump = false;
//         }
//     }

//     // Wall Mechanic

//     public void TouchingWall(bool touching)
//     {
//         if (touching && !touchingGround){
//             clingingWall = true;
//             canWallJump = true;
//             isJumping = false;
            
//             if (horizontalControl != 0) ChangePlayerState(PlayerState.WallSliding);
//         }
//         else {
//             clingingWall = false;
//             canWallJump = false;
//         }
//     }

    
//     public void WallSlide(){
//         if (clingingWall && !touchingGround && horizontalControl != 0){
//             isWallSliding = true;
//             rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Clamp(rb2d.velocity.y, -wallSlidingSpeed, float.MaxValue));
//         }
//         else{
//             isWallSliding = false;
//         }
//     }

//     public void WallJump()
//     {
//         if (isWallSliding && !isWallJumping && verticalControl > 0){
//             CancelInvoke(nameof(StopWallJumping));
//             float wallJumpDirection = -transform.localScale.x;     
//             isWallSliding = false;
//             isWallJumping = true;       
//             canWallJump = false;

//             rb2d.AddForce(new Vector2(wallJumpDirection * wallJumpForce.x, wallJumpForce.y), ForceMode2D.Impulse);
            
//             if (transform.localScale.x != wallJumpDirection){
//                 Vector3 localScale = transform.localScale;
//                 localScale.x *= -1f;
//                 transform.localScale = localScale;
//             }
//             ChangePlayerState(PlayerState.WallJumping);

//             Invoke(nameof(StopWallJumping), wallJumpDuration);
//         }

//     }

//     public void StopWallJumping(){
//         isWallJumping = false;
//     }

//     private void ChangePlayerState(PlayerState newState){
//         switch (newState){
//             case PlayerState.Falling:
//                 if (isWallJumping || isWallSliding){
//                     return;
//                 }
//                 break;
//             case PlayerState.Normal:
//                 if (currentState == PlayerState.Jumping) return;
//                 break;
//         }

//         currentState = newState;
//     }
// }
