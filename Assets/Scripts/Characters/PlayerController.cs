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
    
    private Dictionary<string, Coroutine> buttonReleasedStates = new Dictionary<string, Coroutine>();

    private bool damageInvulnerability = false;

    public event Action<GameObject> OnDestroyEvent;

    protected override void Awake()
    {
        base.Awake();
        playerData.ResetPlayerStats();
        damageInvulnerability = false;
    }


    public PlayerData GetPlayerData(){
        return playerData;
    }

    /// <summary>
    /// Common Method to take Damage for a Characters
    /// </summary>
    /// <param name="damage">Damage to be Done</param>
    public virtual void TakeDamage(int damage, GameObject damagingObject = null, float knockbackForce = 0){
        if (damageInvulnerability) return;
        
        Debug.Assert(damage >= 0, "Damage is less than 0 for some reason: " + this);

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

    public LayerCheck GetAttackCheck(){
        return attackCheck;
    }

    public LayerCheck GetWallCheck(){
        return wallCheck;
    }

    public virtual float GetGrappleControls()
    {
        return Input.GetAxisRaw("Grapple");
    }

    public virtual float GetHorizontalControls(){
        return Input.GetAxisRaw("Horizontal");
    }

    public virtual float GetVerticalControls(){
        return Input.GetAxisRaw("Vertical");
    }

    public virtual float GetDashControls()
    {
        return GetSingularPress("Dash");
    }

    public virtual float GetAttackControls()
    {
        return GetSingularPress("Attack");
    }

    public virtual float GetSprintControls()
    {
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

    
}