using System;
using UnityEngine;

/// <summary>
/// Base Class that all characters in this game must inherit from
/// </summary>
public class PlayerController : BaseCharacter<PlayerController>, IDamageable
{
    [SerializeField] PlayerData playerData;
    [SerializeField] private LayerCheck attackCheck;

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

        Destroy(transform.parent.gameObject);
    }

    public LayerCheck GetAttackCheck(){
        return attackCheck;
    }

    public virtual float GetHorizontalControls(){
        return Input.GetAxisRaw("Horizontal");
    }

    public virtual float GetVerticalControls(){
        return Input.GetAxisRaw("Vertical");
    }

    public virtual float GetDashControls()
    {
        return Input.GetAxisRaw("Dash");
    }

    public virtual float GetAttackControls(){
        return Input.GetAxisRaw("Attack");
    }
}