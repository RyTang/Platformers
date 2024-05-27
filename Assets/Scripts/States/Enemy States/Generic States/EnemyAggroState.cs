using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAggroState : BaseState<BaseEnemy> {
    protected GameObject objToChase;

    protected bool isChasing;
    protected float timer;
    
    
    public override void EnterState(BaseEnemy parent, object objToPass)
    {
        objToChase = (GameObject) objToPass;
        isChasing = true;
        base.EnterState(parent, objToPass);
    }

    public override void UpdateState()
    {
        List<GameObject> objsInRange = Runner.GetDetectCheck().GetObjectsInCheck(); 

        bool objInRange = objsInRange.Contains(objToChase);

        if (objInRange) {
            timer = Runner.GetBasicEnemydata().chaseDuration;
            isChasing = true;
        }

        else if (!objInRange){
            timer -= Time.deltaTime;

            if (timer <= 0){
                isChasing = false;
            }
        }
    }

    public override void CheckStateTransition()
    {
        if (!isChasing){
            Runner.SetMainState(typeof(EnemyMoveToState), Runner.GetBasicEnemydata().startingPosition);
        }
        else if (Runner.GetAttackCheck().Check()){
            // TODO: Attempt to Attack
            Runner.SetMainState(typeof(EnemyAttackState), objToChase);
        }
    }
}
