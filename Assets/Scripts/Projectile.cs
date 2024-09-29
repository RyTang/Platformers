using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile : MonoBehaviour{
    public int projectileDamage = 0;
    public float knockbackForce = 0;
    public float projectileSpeed;

    [SerializeField] private Rigidbody2D rb2d;
    private Vector2 direction;
    private GameObject shooter;

    // private LayerMask colisionMask // TODO: Determine if should use a Layer mask or just use Physics Interpretation

    
    public void Awake() {
        Debug.Assert(GetComponent<Rigidbody2D>() != null, $"{this} object does not contain a Rigidbody");
    }

    public void SetShooter(GameObject shooter) {
        this.shooter = shooter;
    }
    
    // FIXME: Determine on how to Set Projectile Speed, should it be set by the shooter or the bullet itself
    public void SetProjectileTarget(Vector2 positionToTarget){
        direction = ((Vector2) transform.position - positionToTarget).normalized;

        rb2d.velocity = direction * projectileSpeed;
    }

    public void SetProjectileSpeed(float projectileSpeed){
        this.projectileSpeed = projectileSpeed;
    }

    public void OnCollisionEnter2D(Collision2D other) {
        DamageObject(other.gameObject);
    }

    public void OnCollisionStay2D(Collision2D other)
    {
        DamageObject(other.gameObject);
    }

    private void DamageObject(GameObject other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        damageable?.TakeDamage(projectileDamage, shooter, knockbackForce);
    }
}