
using System;
using UnityEngine;

/// <summary>
/// Interface any object that can be affected by damaged should be interfacing this
/// </summary>
public interface IDamageable
{
    event Action<GameObject> OnDestroyEvent;

    /// <summary>
    /// Damage to be taken and applying a knockback force on the target in persepective to the sender
    /// </summary>
    /// <param name="damage">Damage to be Taken</param>
    /// <param name="damagingObject">Object that caused the damage</param>
    /// <param name="knockbackForce">Amount of Knockback force to apply</param>
    void TakeDamage(int damage, GameObject damagingObject = null, float knockbackForce = 0);
    
    /// <summary>
    /// What happens when this objedt gets destroyed
    /// </summary>
    void Destroyed();
}