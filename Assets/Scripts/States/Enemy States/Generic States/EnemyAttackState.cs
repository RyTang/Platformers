using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class EnemyAttackState : BaseState<BaseEnemy>, IAttack{
    protected bool _isAttacking;

    protected Coroutine attackCooldown;

    public bool IsAttacking { get => _isAttacking; set => _isAttacking = value; }

    [HideInInspector] public GameObject objToAttack;

    public override void EnterState(BaseEnemy parent, object objToPass)
    {
        base.EnterState(parent, objToPass);

        objToAttack = objToPass as GameObject;
    }

    public override void EnterState(BaseEnemy parent)
    {
        base.EnterState(parent);

        if (IsAttacking) return;
        
        // Prevent from Moving  
        Runner.GetRigidbody2D().velocity = new Vector2(0, Runner.GetRigidbody2D().velocity.y);
        Attack();   
    }

    public void Attack()
    {
        Runner.StartCoroutine(StartAttack());
    }

    /// <summary>
    /// Causes the Enemy to Attack in front of them
    /// </summary>
    protected abstract IEnumerator StartAttack();

    protected virtual IEnumerator AttackDelay(){
        yield return new WaitForSeconds(Runner.GetBasicEnemydata().attackCooldown);

        IsAttacking = false;
        Runner.StopCoroutine(attackCooldown);
        attackCooldown = null;
    }


    public override void CheckStateTransition()
    {
        if (!IsAttacking){
            Runner.SetMainState(typeof(EnemyAggroState), objToAttack);
        }
    }
}