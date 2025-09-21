using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Sub Attack 3")]
public class GroundSubAttackThree : BaseState<PlayerController>, IAttack
{
    private float attackControl;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        // Prevent from Moving  
        Runner.GetRigidbody2D().velocity = new Vector2(0, Runner.GetRigidbody2D().velocity.y);
        
        Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerGroundAttack03);

        Runner.GetAnimator().SetBool(PlayerAnimation.isAttackingBool, true);
    }

    public void Attack()
    {
        List<GameObject> attackedObjects = Runner.GetAttackCheck().GetObjectsInCheck();

        foreach (GameObject attackedObject in attackedObjects)
        {
            IDamageable damageable;
            bool attackable = attackedObject.TryGetComponent(out damageable);
            // TODO: Need to adjust damage points
            if (attackable) damageable.TakeDamage(Runner.GetPlayerData().attackDamage, Runner.gameObject, Runner.GetPlayerData().knockbackForce);
        }
    }

    public override void CaptureInput()
    {
        // If attack triggered then just keep as queued until able to attack
        if (attackControl <= 0){
            attackControl = Runner.GetAttackControls();        
        }
    }

    public override void CheckStateTransition()
    {
        if (attackControl > 0 && Runner.GetAnimator().GetBool(PlayerAnimation.canAttackBool) && Runner.GetAnimator().GetBool(PlayerAnimation.isAttackingBool)){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(GroundSubAttackOne)));
        }
        else if (attackControl <= 0 && !Runner.GetAnimator().GetBool(PlayerAnimation.isAttackingBool)){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalIdleState)));
        }
    }

    public override IEnumerator ExitState()
    {
        attackControl = 0;
        yield break;
    }
}