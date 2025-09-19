using UnityEngine;

/// <summary>
/// Base Class that all characters in this game must inherit from
/// </summary>
public abstract class BaseCharacter<T> : StateRunner<T> where T: MonoBehaviour
{
    protected Rigidbody2D rb2d;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    /// <summary>
    /// If true, the character will face the direction of movement. Set to false if you want to control direction manually
    /// </summary>
    protected bool canChangeDirection = true;

    
    [SerializeField] protected LayerCheck groundCheck;



    protected override void Awake()
    {
        rb2d ??= GetComponent<Rigidbody2D>();
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        animator ??= GetComponent<Animator>();
    }

    public override void Update()
    {
        base.Update();
        if (canChangeDirection)
        {
            SpriteDirection();
        }
    }
    
    /// <summary>
    /// Toggles on whether sprite direction can change normally, can be used in situations like ledge climbing where you don't want the player to change direction. Etc if player is attacked and u don't want them to rotate
    /// </summary>
    /// <param name="canChangeDirection">Whether they can change direction normally</param>
    public virtual void CanRotate(bool canChangeDirection)
    {
        this.canChangeDirection = canChangeDirection;
    }


    /// <summary>
    /// Faces the direction of the rb2d velocity, override it to prevent animation that is based on this
    /// </summary>
    protected virtual void SpriteDirection()
    {
        Vector3 localScale = spriteRenderer.transform.localScale;

        if (rb2d.velocity.x > 0)
        {
            spriteRenderer.transform.localScale = new Vector3(Mathf.Abs(localScale.x), localScale.y, localScale.z);
        }
        else if (rb2d.velocity.x < 0)
        {
            spriteRenderer.transform.localScale = new Vector3(-Mathf.Abs(localScale.x), localScale.y, localScale.z);
        }
    }

    public Rigidbody2D GetRigidbody2D(){
        return rb2d;
    }

    public SpriteRenderer GetSpriteRenderer(){
        return spriteRenderer;
    }

    public LayerCheck GetGroundCheck(){
        return groundCheck;
    }

    

    public Animator GetAnimator(){
        return animator;
    }
}