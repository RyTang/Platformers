using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class FlyingEnemyGrunt : BaseEnemy
{
    [Header("Shooting Enemy")]
    public GameObject bulletPrefab;
    public float projectileSpeed;
    public int projectileDamage;
}