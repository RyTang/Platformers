using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public interface IShootable
{
    public GameObject BulletPrefab {get; set;}
    public float ProjectileSpeed {get; set;}
    public int ProjectileDamage {get; set;}

    public void Shoot(Transform objectToShoot);

    public void Shoot(Vector2 positionToShoot);
}