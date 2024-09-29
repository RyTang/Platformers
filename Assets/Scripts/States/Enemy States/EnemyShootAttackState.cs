using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy State/Enemy Shoot Attack State")]
public class EnemyShootAttackState : EnemyAttackState, IShootable
{
    [Header("Shooting Enemy")]
    private GameObject bulletPrefab;
    public float projectileSpeed;
    public int projectileDamage;
    
    public GameObject BulletPrefab { get => bulletPrefab; set => bulletPrefab = value; }
    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public int ProjectileDamage { get => projectileDamage; set => projectileDamage = value; }


    private BaseEnemy parent;

    public override void EnterState(BaseEnemy parent)
    {
        this.parent = parent;
        base.EnterState(parent);
    }

    public override void FixedUpdateState()
    {

    }

    public void Shoot(Transform objectToShoot)
    {
        Shoot(objectToShoot.position);
    }

    public void Shoot(Vector2 positionToShoot)
    {
        // Instanstiate bullet, and add velocity to the bullet
        bulletPrefab = Instantiate(bulletPrefab, parent.transform.position, Quaternion.identity, parent.transform);


        Projectile bulletRb = bulletPrefab.GetComponent<Projectile>();
        bulletRb.SetShooter(parent.gameObject);
        bulletRb.SetProjectileTarget(positionToShoot);
    }

    public override void UpdateState()
    {
    }

    protected override IEnumerator StartAttack()
    {
        IsAttacking = true;

        yield return new WaitForSeconds(Runner.GetBasicEnemydata().attackTime);

        // Shoot Projectile

        // Attack at the actual target
        Shoot(objToAttack.transform);

        if (IsAttacking) attackCooldown = Runner.StartCoroutine(AttackDelay());

        /// TODO: Try to change the normal attack states to follow this pattern as well. Have Generic States, and ask the Player/Enemy to return back their attack pattern. -> Does this make sense?
    }
}