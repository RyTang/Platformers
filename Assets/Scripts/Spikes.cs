using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public int damage;
    public float knockbackForce = 1f;


    public void OnCollisionEnter2D(Collision2D other) {
        Debug.Log($"Touching {other.gameObject}");
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();

        if (damageable != null) {
            damageable.TakeDamage(damage, gameObject, knockbackForce);
        }
    }
}
