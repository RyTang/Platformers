using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy State/Enemy Melee Attack State")]
public class EnemyMeleeAttackState : EnemyAttackState
{
    protected override IEnumerator StartAttack()
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
            if (attackable) damageable.TakeDamage(Runner.GetBasicEnemydata().attackDamage, Runner.gameObject, Runner.GetBasicEnemydata().knockbackForce);
        }


        if (IsAttacking) attackCooldown = Runner.StartCoroutine(AttackDelay());
    }

    public override void CaptureInput()
    {
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