using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAttackState : State<PlayerController>
{
    protected bool attacking;
    protected float attackControl;
    protected Coroutine attackDelay;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        // Prevent from Moving  
        _runner.GetRigidbody2D().velocity = new Vector2(0, _runner.GetRigidbody2D().velocity.y);
        Attack();
    }

    protected virtual void Attack()
    {
        if (attackDelay != null)
        {
            _runner.StopCoroutine(attackDelay);
            attackDelay = null;
        }

        AttackAnimation();

        attacking = true;
        AttackCollision();

        if (attacking) attackDelay = _runner.StartCoroutine(AttackDelay());
    }

    protected virtual void AttackCollision()
    {
        List<GameObject> attackedObjects = _runner.GetAttackCheck().GetObjectsInCheck();

        foreach (GameObject attackedObject in attackedObjects)
        {
            bool attackable = attackedObject.TryGetComponent(out IDamageable damageable);
            if (attackable) damageable.TakeDamage(_runner.GetPlayerData().attackDamage);
        }
    }

    /// <summary>
    /// Animations to Trigger when attacking
    /// </summary>
    protected abstract void AttackAnimation();


    /// <summary>
    /// The Pause for the attack
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator AttackDelay(){
        
        yield return new WaitForSeconds(_runner.GetPlayerData().attackTime);
        
        if (attackControl > 0){
            // TODO: Switch to next attack
            NextAttack();
        }
        else{
            _runner.GetAnimator().SetBool(PlayerAnimation.isAttackingBool, false);
            attacking = false;
        }        
    }

    /// <summary>
    /// Queues the next attack if control is pressed during current attack animation
    /// </summary>
    public abstract void NextAttack();

    public override void CaptureInput()
    {
        attackControl = _runner.GetAttackControls();

    }

    public override void ChangeState()
    {
        if (!attacking){
            _runner.SetState(typeof(WalkState));
        }
    }

    public override void ExitState()
    {
        attacking = false;
        attackDelay = null;
    }

    public override void FixedUpdate()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void Update()
    {
    }
}