using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyShootAttackState : EnemyAttackState
{
    IShootable shootingStats;

    public override void EnterState(BaseEnemy parent)
    {
        base.EnterState(parent);

        shootingStats =  parent.GetComponent<IShootable>();
        Debug.Assert(shootingStats != null, "${parent} does not contain any IShootable stats for attack");
    }

    public override void FixedUpdateState()
    {

    }

    public override void UpdateState()
    {
    }

    protected override IEnumerator StartAttack()
    {
        IsAttacking = true;

        yield return new WaitForSeconds(Runner.GetBasicEnemydata().attackTime);

        // Shoot Projectile
        Debug.Assert(shootingStats != null, "${parent} tried to Attack without IShootable stats");
        if (shootingStats == null) yield break;;

        
        // Attack at the actual target
        shootingStats.Shoot(objToAttack.transform);

        if (IsAttacking) attackCooldown = Runner.StartCoroutine(AttackDelay());

        /// TODO: Try to change the normal attack states to follow this pattern as well. Have Generic States, and ask the Player/Enemy to return back their attack pattern. -> Does this make sense?
    }
}