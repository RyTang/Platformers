using UnityEngine;

/// <summary>
/// Base Class that all characters in this game must inherit from
/// </summary>
public abstract class BaseCharacter : MonoBehaviour, IDamageable
{
    [SerializeField] protected int health;
    protected Rigidbody2D rb2d;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Start()
    {
        rb2d ??= GetComponent<Rigidbody2D>();
        spriteRenderer ??= GetComponent<SpriteRenderer>();

        // TODO : Decide if should store character info in a character configuration
    }

    protected virtual void Update()
    {
        SpriteDirection();
    }

    protected virtual void FixedUpdate()
    {
        
    }

    protected virtual void SpriteDirection(){
        Vector3 localScale =  spriteRenderer.transform.localScale;

        if (rb2d.velocity.x > 0) {
            spriteRenderer.transform.localScale = new Vector3(Mathf.Abs(localScale.x), localScale.y, localScale.z);
        }
        else if (rb2d.velocity.x < 0){
            spriteRenderer.transform.localScale = new Vector3(-Mathf.Abs(localScale.x), localScale.y, localScale.z);
        }
    }


    /// <summary>
    /// Common Method to take Damage for a Characters
    /// </summary>
    /// <param name="damage">Damage to be Done</param>
    public virtual void TakeDamage(int damage){
        Debug.Assert(damage >= 0, "Damage is less than 0 for some reason: " + this);

        if (damage < 0) return;

        health -= damage;
    }

    /// <summary>
    /// Cause the Character to die
    /// </summary>
    public virtual void Destroyed(){
        // TODO: Perform Death Animation

        Destroy(gameObject);
    }
}