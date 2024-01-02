using UnityEngine;

[CreateAssetMenu(menuName = "State/Ground Attack/Ground Attack 2")]
public class GroundAttackState2 : BaseAttackState
{
    public override void NextAttack()
    {
        if (attackControl > 0 ) {
            _runner.SetState(typeof(GroundAttackState1));
        }
    }

    protected override void AttackAnimation()
    {
        _runner.GetAnimator().SetTrigger(PlayerAnimation.triggerGroundAttack02);

        _runner.GetAnimator().SetBool(PlayerAnimation.isAttackingBool, true);
    }
}