using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour, IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private Rigidbody2D rb2d;

    public event Action<GameObject> OnDestroyEvent;
    
    public virtual void Destroyed()
    {
        OnDestroyEvent?.Invoke(gameObject); // TODO: Figure out what is the point of having this again
        Destroy(transform.root.gameObject);
    }

    public virtual void TakeDamage(int damage, GameObject damagingObject = null, float knockbackForce = 0)
    {
        // Apply knockback
        if (damagingObject != null){
            Vector2 directionDifference = (transform.position - damagingObject.transform.position).normalized;

            rb2d.AddForce(directionDifference * knockbackForce, ForceMode2D.Impulse);
        }

        if (damage < 0) return;


        health = Mathf.Max(health - damage, 0);

        if (health <= 0){
            Destroyed();
        }
    }
}
