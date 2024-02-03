using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : BaseCharacter<BaseEnemy>, IDamageable
{
    [SerializeField] private LayerCheck _detectionArea; 
    [SerializeField] private LayerCheck _attackArea;
    [SerializeField] private int _health = 2;

    [SerializeField] BasicEnemyData enemyData;

    public event Action<GameObject> OnDestroyEvent;

    protected int Health { get => _health; set => _health = value; }

    protected override void Awake()
    {
        base.Awake();

        enemyData = Instantiate(enemyData);
    }

    protected override void Start()
    {
        base.Start();

        Health = enemyData.health;
    }

    public void Destroyed()
    {
        OnDestroyEvent.Invoke(gameObject);
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

    public LayerCheck GetDetectCheck(){
        return _detectionArea;
    }

    public LayerCheck GetAttackCheck(){
        return _attackArea;
    }

    public BasicEnemyData GetBasicEnemydata(){
        return enemyData;
    }
}
