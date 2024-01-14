using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy<T> : BaseCharacter<T>, IDamageable where T: MonoBehaviour
{
    [SerializeField] private int health = 2;

    protected int Health { get => health; set => health = value; }

    public void Destroyed()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0 ) return;

        Health -= damage;
        if (Health <= 0){
            Destroyed();
        }
    }
}
