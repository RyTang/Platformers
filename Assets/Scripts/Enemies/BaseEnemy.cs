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

    [SerializeField] protected SimpleFlash injuredFlash;

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
        enemyData.startingPosition = transform.position;
    }

    public void Destroyed()
    {
        OnDestroyEvent?.Invoke(gameObject);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage, GameObject damagingObject = null, float knockbackForce = 0)
    {
        // Apply knockback
        if (damagingObject != null){
            Vector2 directionDifference = (transform.position - damagingObject.transform.position).normalized;

            rb2d.AddForce(directionDifference * knockbackForce, ForceMode2D.Impulse);
        }

        if (damage < 0 ) return;

        GetInjuredFlash().Flash(1f); // TODO: Make duration a variable

        Health -= damage;
        if (Health <= 0){
            Destroyed();
        }
    }
    
    public SimpleFlash GetInjuredFlash(){
        return injuredFlash;
    }

    public LayerCheck GetDetectCheck()
    {
        return _detectionArea;
    }

    public LayerCheck GetAttackCheck(){
        return _attackArea;
    }

    public BasicEnemyData GetBasicEnemydata(){
        return enemyData;
    }
}
