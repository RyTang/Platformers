using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Sub Attack 1")]
public class GroundSubAttackOne : BaseState<PlayerController>, IAttack
{
    private float attackControl;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        // Prevent from Moving  
        Runner.GetRigidbody2D().velocity = new Vector2(0, Runner.GetRigidbody2D().velocity.y);

        // How to trigger the Attack Method in that time
        Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerGroundAttack01);

        Runner.GetAnimator().SetBool(PlayerAnimation.isAttackingBool, true);
    }

    public void Attack()
    {
        List<GameObject> attackedObjects = Runner.GetAttackCheck().GetObjectsInCheck();

        foreach (GameObject attackedObject in attackedObjects)
        {
            IDamageable damageable;
            bool attackable = attackedObject.TryGetComponent<IDamageable>(out damageable);
            // TODO: Need to adjust damage points
            if (attackable) damageable.TakeDamage(Runner.GetPlayerData().attackDamage, Runner.gameObject, Runner.GetPlayerData().knockbackForce);
        }

    }


    public override IEnumerator ExitState()
    {
        attackControl = 0;
        yield break;
    }

    public override void CaptureInput()
    {
        // If attack triggered then just keep as queued until able to attack
        if (attackControl <= 0){
            // TODO: Add in input Correction where if input is triggered not within a few milli seconds of it ending then don't trigger
            attackControl = Runner.GetAttackControls();        
        }  
    }

    public override void CheckStateTransition()
    {
        if (attackControl > 0 && Runner.GetAnimator().GetBool(PlayerAnimation.canAttackBool) && Runner.GetAnimator().GetBool(PlayerAnimation.isAttackingBool)){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(GroundSubAttackTwo)));
        }
        else if (attackControl <= 0 && !Runner.GetAnimator().GetBool(PlayerAnimation.isAttackingBool)){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalIdleState)));
        }
    }

}