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
    [SerializeField] private LayerCheck attackCheck;
    [SerializeField] protected LayerCheck wallCheck;
    
    private Dictionary<string, Coroutine> buttonReleasedStates = new Dictionary<string, Coroutine>();

    protected override void Awake()
    {
        base.Awake();
        playerData = Instantiate(playerData);
        playerData.currentEnergy = playerData.maxEnergyBar;
    }


    public PlayerData GetPlayerData(){
        return playerData;
    }

    /// <summary>
    /// Common Method to take Damage for a Characters
    /// </summary>
    /// <param name="damage">Damage to be Done</param>
    public virtual void TakeDamage(int damage){
        Debug.Assert(damage >= 0, "Damage is less than 0 for some reason: " + this);

        if (damage < 0) return;

        playerData.health -= damage;
        SetMainState(typeof(InjuredState));
        if (playerData.health <= 0 ) Destroyed();
    }

    /// <summary>
    /// Cause the Character to die
    /// </summary>
    public virtual void Destroyed(){
        // TODO: Perform Death Animation

        Destroy(transform.root.gameObject);
    }

    public LayerCheck GetAttackCheck(){
        return attackCheck;
    }

    public LayerCheck GetWallCheck(){
        return wallCheck;
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
        if (!buttonReleasedStates.ContainsKey(buttonToRelease)) yield break;

        while (Input.GetAxisRaw(buttonToRelease) != 0){
            yield return null;
        }

        buttonReleasedStates.Remove(buttonToRelease);
    }

    
}