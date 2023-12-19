using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Ground Attack")]
public class GroundAttackState : State<PlayerController>
{
    private bool attacking;
    private float attackControl;
    private Coroutine attackDelay;

    public override void EnterState(PlayerController parent)
    {

        base.EnterState(parent);

        // Prevent from Moving  
        _runner.GetRigidbody2D().velocity = new Vector2(0, _runner.GetRigidbody2D().velocity.y);
        Attack();
    }

    private void Attack()
    {
        if (attackDelay != null)
        {
            _runner.StopCoroutine(attackDelay);
            attackDelay = null;
        }

        _runner.GetAnimator().SetTrigger(PlayerAnimation.attackTrigger);

        _runner.GetAnimator().SetBool(PlayerAnimation.isAttackingBool, true);

        attacking = true;

        List<GameObject> attackedObjects = _runner.GetAttackCheck().GetObjectsInCheck();

        foreach (GameObject attackedObject in attackedObjects)
        {
            IDamageable damageable;
            bool attackable = attackedObject.TryGetComponent<IDamageable>(out damageable);
            if (attackable) damageable.TakeDamage(_runner.GetPlayerData().attackDamage);
        }


        if (attacking) attackDelay = _runner.StartCoroutine(AttackDelay());
    }

    private IEnumerator AttackDelay(){
        
        yield return new WaitForSeconds(_runner.GetPlayerData().attackTime);
        
        if (attackControl > 0){
            Attack();
        }
        else{
            _runner.GetAnimator().SetBool(PlayerAnimation.isAttackingBool, false);
            attacking = false;
        }        
    }

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