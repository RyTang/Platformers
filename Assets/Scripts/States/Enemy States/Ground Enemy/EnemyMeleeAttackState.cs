using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy State/Enemy Melee Attack State")]
public class EnemyMeleeAttackState : EnemyAttackState
{
    private bool _isAttacking;

    private Coroutine attackCooldown;

    public bool IsAttacking { get => _isAttacking; set => _isAttacking = value; }

    public override void EnterState(BaseEnemy parent)
    {
        base.EnterState(parent);

        if (IsAttacking) return;
        
        // Prevent from Moving  
        Runner.GetRigidbody2D().velocity = new Vector2(0, Runner.GetRigidbody2D().velocity.y);
        
        Runner.StartCoroutine(StartAttack());
    }

    private IEnumerator StartAttack()
    {
        IsAttacking = true;
        // TODO: Play Animation if Needed
        // TODO: Decide if animation will dictate how long it should play, or the code -> If code need to segment animation to windup and attack animation
        yield return new WaitForSeconds(Runner.GetBasicEnemydata().attackTime);

        List<GameObject> attackedObjects = Runner.GetAttackCheck().GetObjectsInCheck();

        foreach (GameObject attackedObject in attackedObjects)
        {
            IDamageable damageable;
            bool attackable = attackedObject.TryGetComponent<IDamageable>(out damageable);
            if (attackable) damageable.TakeDamage(Runner.GetBasicEnemydata().attackDamage);
        }


        if (IsAttacking) attackCooldown = Runner.StartCoroutine(AttackDelay());
    }

    private IEnumerator AttackDelay(){
        
        yield return new WaitForSeconds(Runner.GetBasicEnemydata().attackCooldown);

        IsAttacking = false;
        attackCooldown = null;
    }

    public override void CaptureInput()
    {
    }

    public override void CheckStateTransition()
    {
        if (!IsAttacking){
            Runner.SetMainState(typeof(EnemyDetectTargetState));
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void InitialiseSubState()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void UpdateState()
    {
    }
    
    public override void OnStateCollisionStay(Collision2D collision)
    {
    }

    public override void OnStateCollisionExit(Collision2D collision)
    {
    }
}