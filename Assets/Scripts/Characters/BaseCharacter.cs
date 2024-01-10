using UnityEngine;

/// <summary>
/// Base Class that all characters in this game must inherit from
/// </summary>
public abstract class BaseCharacter<T> : StateRunner<T> where T: MonoBehaviour
{
    protected Rigidbody2D rb2d;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    
    [SerializeField] protected LayerCheck groundCheck;
    [SerializeField] protected LayerCheck wallCheck;



    protected override void Awake()
    {
        rb2d ??= GetComponent<Rigidbody2D>();
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        animator ??= GetComponent<Animator>();
    }

    public override void Update()
    {
        base.Update();

        SpriteDirection();
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

    public Rigidbody2D GetRigidbody2D(){
        return rb2d;
    }

    public SpriteRenderer GetSpriteRenderer(){
        return spriteRenderer;
    }

    public LayerCheck GetGroundCheck(){
        return groundCheck;
    }

    public LayerCheck GetWallCheck(){
        return wallCheck;
    }

    public Animator GetAnimator(){
        return animator;
    }
}