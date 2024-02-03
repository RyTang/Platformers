using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour, IDamageable
{
    [SerializeField] private int health;

    public event Action<GameObject> OnDestroyEvent;

    public virtual void Destroyed()
    {
        OnDestroyEvent(gameObject);
        Destroy(transform.root.gameObject);
    }

    public virtual void TakeDamage(int damage)
    {
        if (damage < 0) return;

        health = Mathf.Max(health - damage, 0);

        if (health <= 0){
            Destroyed();
        }
    }
}
