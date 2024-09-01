using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class FlyingEnemyGrunt : BaseEnemy, IShootable
{
    [Header("Shooting Enemy")]
    public GameObject bulletPrefab;
    public float projectileSpeed;
    public int projectileDamage;
    
    public GameObject BulletPrefab { get => bulletPrefab; set => bulletPrefab = value; }
    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public int ProjectileDamage { get => projectileDamage; set => projectileDamage = value; }

    public void Shoot(Transform objectToShoot)
    {
        throw new NotImplementedException();
    }

    public void Shoot(Vector2 positionToShoot)
    {
        throw new NotImplementedException();
    }
}