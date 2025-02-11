using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Class that all characters in this game must inherit from
/// </summary>
public class PlayerController : BaseCharacter<PlayerController>, IDamageable
{
    [SerializeField] PlayerData playerData;
    [SerializeField] protected GameEvent playerHurtEvent;
    [SerializeField] protected GameEvent playerDeathEvent;
    [SerializeField] private LayerCheck attackCheck;
    [SerializeField] protected LayerCheck wallCheck;
    [SerializeField] protected LayerCheck ledgeCheck;

    public delegate void OnAnimationEventTriggered(AnimationEventTrigger eventTrigger);
    public event OnAnimationEventTriggered AnimationEvent;
    public bool canRotate = true;

    private Dictionary<string, Coroutine> buttonReleasedStates = new Dictionary<string, Coroutine>();

    private bool damageInvulnerability = false;
    private bool disableVerticalControls = false, disableHorizontalControls = false;
    

    public event Action<GameObject> OnDestroyEvent;

    protected override void Awake()
    {
        base.Awake();
        playerData.ResetPlayerStats();
        rb2d.gravityScale = playerData.gravityScale;
        damageInvulnerability = false;
        canRotate = true;
    }

    protected override void SpriteDirection()
    {
        // So that knockback doesn't affect the direction that the player is facing and that it is only based on controls;
        float xDirection = GetHorizontalControls();
        if (xDirection == 0 || !canRotate) return;

        Vector3 localScale =  spriteRenderer.transform.localScale;

        float facingDirection = xDirection < 0 ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);

        spriteRenderer.transform.localScale = new Vector3(facingDirection, localScale.y, localScale.z);
    }


    public PlayerData GetPlayerData(){
        return playerData;
    }

    public void AnimationUpdateEvent(string animationEvent){
        if (Enum.TryParse(animationEvent, true, out AnimationEventTrigger animationEventTrigger)){
            AnimationEvent?.Invoke(animationEventTrigger);
        }
        else {
            Debug.LogError($"Unable to Determine Enum of Type: {animationEvent}");
        }

    }

    /// <summary>
    /// Common Method to take Damage for a Characters
    /// </summary>
    /// <param name="damage">Damage to be Done</param>
    public virtual void TakeDamage(int damage, GameObject damagingObject = null, float knockbackForce = 0){
        if (damageInvulnerability) return;
        
        Debug.Assert(damage >= 0, "Damage is less than 0 for some reason: " + this);
        if (damage == 0){            
            Debug.LogWarning("Damage == 0 for some reason: " + this);
        }

        if (damagingObject != null) {
            // Add Directional Knockback
            Vector2 directionDifference = (transform.position - damagingObject.transform.position).normalized;

            rb2d.AddForce(directionDifference * knockbackForce, ForceMode2D.Impulse);
        }


        if (damage < 0) return;
        
        playerData.health -= damage;
        playerHurtEvent.TriggerEvent();

        StartCoroutine(DamageInvulnerability());
        
        SetMainState(typeof(InjuredState));
        if (playerData.health <= 0 ) Destroyed();
    }

    /// <summary>
    /// Cause the Character to die
    /// </summary>
    public virtual void Destroyed(){
        // TODO: Perform Death Animation
        OnDestroyEvent?.Invoke(gameObject);
        // Destroy(transform.root.gameObject);
        Debug.Log("Got Destroyed");
        playerDeathEvent.TriggerEvent();
    }

    public void IsAttacking(){
        GetAnimator().SetBool(PlayerAnimation.canAttackBool, false);
    }

    public virtual void InitiateAttackHitEvent(){        
        BaseState<PlayerController> finalLeafState = active_state.GetFinalLeafState();
        if (finalLeafState is IAttack attack) {
            attack.Attack();
        }
    }

    public void canMoveToNextAttack(){
        GetAnimator().SetBool(PlayerAnimation.canAttackBool, true);
    }

    public void AttackDone(){
        GetAnimator().SetBool(PlayerAnimation.isAttackingBool, false);
    }

    public LayerCheck GetAttackCheck(){
        return attackCheck;
    }

    public LayerCheck GetWallCheck(){
        return wallCheck;
    }

    public LayerCheck GetLedgeCheck(){
        return ledgeCheck;
    }

    public virtual float GetGrappleControls()
    {
        return Input.GetAxisRaw("Grapple");
    }

    public virtual float GetHorizontalControls(){
        if (disableHorizontalControls) return 0;
        return Input.GetAxisRaw("Horizontal");
    }

    public virtual float GetVerticalControls(){
        if (disableVerticalControls) return 0;
        return Input.GetAxisRaw("Vertical");
    }
    
    public virtual float GetMobilityControl(){
        if (disableHorizontalControls) return 0;
        return GetSingularPress("Mobility");
    }

    public virtual float GetDashControls()
    {
        if (disableHorizontalControls) return 0;
        return GetSingularPress("Dash");
    }

    public virtual float GetAttackControls()
    {
        return GetSingularPress("Attack");
    }

    public virtual float GetSprintControls()
    {
        if (disableHorizontalControls) return 0;
        return Input.GetAxisRaw("Sprint");
    }

    // FIXME: PROBLEM WHERE IF CALLING FROM MAIN STATE AND SUB STATE, MAIN STATE WILL BE PRIOTISED
    private float GetSingularPress(string axisToCheck)
    {
        if (buttonReleasedStates.ContainsKey(axisToCheck))
        {
            return 0;
        }
        float axisValue = Input.GetAxisRaw(axisToCheck);

        if (axisValue > 0){
            buttonReleasedStates.Add(axisToCheck, StartCoroutine(ReleasedButtonPress(axisToCheck)));
        }       
        return axisValue; 
    }

    // Creating code that will return based on instance calling for code
    

    private IEnumerator ReleasedButtonPress(string buttonToRelease){
        if (buttonReleasedStates.ContainsKey(buttonToRelease)) yield break;

        while (Input.GetAxisRaw(buttonToRelease) != 0){
            yield return null;
        }

        buttonReleasedStates.Remove(buttonToRelease);
    }

    private IEnumerator DamageInvulnerability(){
        damageInvulnerability = true;

        yield return new WaitForSeconds(playerData.injuredDuration);

        damageInvulnerability = false;
    }

    /// <summary>
    /// Disable Horizontal Controls
    /// </summary>
    public void DisableHorizontalControls() {
        disableHorizontalControls = true;
    }

    /// <summary>
    /// Enable Horizontal Controls
    /// </summary>
    public void EnableHorizontalControls() {
        disableHorizontalControls = false;
    }

    /// <summary>
    /// Disable Vertical Controls
    /// </summary>
    public void DisableVerticalControls() {
        disableVerticalControls = true;
    }

    /// <summary>
    /// Enable Vertical Controls
    /// </summary>
    public void EnableVerticalControls() {
        disableVerticalControls = false;
    }
}