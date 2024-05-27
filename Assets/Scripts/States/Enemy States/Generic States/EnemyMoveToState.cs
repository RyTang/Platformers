using UnityEngine;

public abstract class EnemyMoveToState: BaseState<BaseEnemy>{
    protected Vector3 positionToMoveTo;
    protected bool reachedDestination;
    protected float tolerance = 0.5f;
    
    public override void EnterState(BaseEnemy parent, object objToPass)
    {
        positionToMoveTo = (Vector3) objToPass;
        reachedDestination = false;

        base.EnterState(parent, objToPass);
    }

    public override void UpdateState()
    {
        float distance = Mathf.Abs(Vector2.Distance(Runner.transform.position, positionToMoveTo));

        if (distance < tolerance){
            reachedDestination = true;
            Runner.GetRigidbody2D().velocity = Vector2.zero;
        }
    }

    public override void CheckStateTransition()
    {
        if (reachedDestination){
            Runner.SetMainState(typeof(EnemyIdleState));
        }
        else if (Runner.GetDetectCheck().Check()){
            Runner.SetMainState(typeof(EnemyAggroState));
        }
    }
}