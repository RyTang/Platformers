using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Ground Attack/Ground Attack 1")]
public class GroundAttackState1 : BaseAttackState
{
    public override void NextAttack()
    {
        if (attackControl > 0 ) {
            _runner.SetState(typeof(GroundAttackState2));
        }
    }


    protected override void AttackAnimation()
    {
        _runner.GetAnimator().SetTrigger(PlayerAnimation.triggerGroundAttack01);

        _runner.GetAnimator().SetBool(PlayerAnimation.isAttackingBool, true);
    }
}