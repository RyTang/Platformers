using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Jump Attack 1")]
public class NormalJumpAttack : BaseState<PlayerController>
{
    private bool _isAttacking;
    private float attackControl;
    private Coroutine attackDelay;
    
    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        // Stop Temporarily from moving
        Runner.GetRigidbody2D().velocity = Vector2.zero;

        Attack();
    }

    private void Attack()
    {
        if (attackDelay != null)
        {
            Runner.StopCoroutine(attackDelay);
            attackDelay = null;
        }

        Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerGroundAttack01);

        Runner.GetAnimator().SetBool(PlayerAnimation.isAttackingBool, true);

        _isAttacking = true;

        List<GameObject> attackedObjects = Runner.GetAttackCheck().GetObjectsInCheck();

        foreach (GameObject attackedObject in attackedObjects)
        {
            IDamageable damageable;
            bool attackable = attackedObject.TryGetComponent<IDamageable>(out damageable);
            if (attackable) damageable.TakeDamage(Runner.GetPlayerData().attackDamage, Runner.gameObject, Runner.GetPlayerData().knockbackForce);
        }


        if (_isAttacking) attackDelay = Runner.StartCoroutine(AttackDelay());
    }

    private IEnumerator AttackDelay(){
        
        yield return new WaitForSeconds(Runner.GetPlayerData().attackTime);

        Runner.GetAnimator().SetBool(PlayerAnimation.isAttackingBool, false);
        _isAttacking = false;       
    }

    public override IEnumerator ExitState()
    {
        while (_isAttacking) {
            yield return null;
        }
        attackControl = 0;
    }

    public override void CheckStateTransition()
    {
        if (!Runner.GetGroundCheck().Check()){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFallState)));
        }
        else if (attackControl <= 0 && !_isAttacking){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalIdleState)));
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void UpdateState()
    {
    }
}