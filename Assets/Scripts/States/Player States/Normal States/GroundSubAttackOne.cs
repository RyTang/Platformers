using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Sub Attack 1")]
public class GroundSubAttackOne : BaseState<PlayerController>
{
    private bool _isAttacking;
    private float attackControl;
    private Coroutine attackDelay;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        Debug.Log("Attacking");

        // Prevent from Moving  
        Runner.GetRigidbody2D().velocity = new Vector2(0, Runner.GetRigidbody2D().velocity.y);

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

    public override void CaptureInput()
    {
        // FIXME: Attack Controls continuosly staying as positive
        attackControl = Runner.GetAttackControls();        
    }

    public override void CheckStateTransition()
    {
        if (attackControl > 0 && _isAttacking){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(GroundSubAttackTwo)));
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