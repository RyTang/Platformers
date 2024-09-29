using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile : MonoBehaviour{
    private int projectileDamage = 0;
    private float knockbackForce = 0;
    private float projectileSpeed;

    private List<GameObject> damagedObjects = new List<GameObject>();

    [SerializeField] private Rigidbody2D rigidBody2D;
    private Vector2 direction;
    private GameObject shooter;

    // private LayerMask colisionMask // TODO: Determine if should use a Layer mask or just use Physics Interpretation

    
    public virtual void Awake() {
        Debug.Assert(GetComponent<Rigidbody2D>() != null, $"{this} object does not contain a Rigidbody");
    }

    public virtual void SetProjectileTarget(GameObject shooter, Vector2 positionToTarget, int projectileDamage, float projectileSpeed, float knockbackForce = 0f) {
        direction = (positionToTarget - (Vector2) transform.position).normalized;

        rigidBody2D.velocity = direction * projectileSpeed;
        this.projectileDamage = projectileDamage;
        this.projectileSpeed = projectileSpeed;
        this.knockbackForce = knockbackForce;
    }

    // TODO: How to care for explosion kind of Projectiles, do they just extend from this behaviour
    public virtual void OnCollisionEnter2D(Collision2D other) {
        DamageObject(other.gameObject);
        if (gameObject != null){
            Destroy(gameObject);
        }
    }

    protected virtual void DamageObject(GameObject other)
    {
        if (other != null && damagedObjects.Contains(other)) return;


        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        damageable?.TakeDamage(projectileDamage, shooter, knockbackForce);
        damagedObjects.Add(other);
    }
}