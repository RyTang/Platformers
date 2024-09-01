using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Jump Attack 1")]
public class NormalJumpAttack : BaseState<PlayerController>, IAttack
{
    private bool _isAttacking;
    private float attackControl;
    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        // Stop Temporarily from moving
        Runner.GetRigidbody2D().velocity = Vector2.zero;

        Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerGroundAttack01);

        Runner.GetAnimator().SetBool(PlayerAnimation.isAttackingBool, true);
    }

    public void Attack()
    {
        _isAttacking = true;

        List<GameObject> attackedObjects = Runner.GetAttackCheck().GetObjectsInCheck();

        foreach (GameObject attackedObject in attackedObjects)
        {
            IDamageable damageable;
            bool attackable = attackedObject.TryGetComponent(out damageable);
            if (attackable) damageable.TakeDamage(Runner.GetPlayerData().attackDamage, Runner.gameObject, Runner.GetPlayerData().knockbackForce);
        }


    }

    public override IEnumerator ExitState()
    {
        attackControl = 0;
        yield break;
    }
    

    public override void CheckStateTransition()
    {
        if (!Runner.GetAnimator().GetBool(PlayerAnimation.isAttackingBool)){
            if (!Runner.GetGroundCheck().Check()){
                CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalFallState)));
            }
            else if (attackControl <= 0){
                CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalIdleState)));
            }
        }
    }

}