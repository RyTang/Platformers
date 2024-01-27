using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy State/Detect Target")]
public class EnemyDetectTargetState : BaseState<BaseEnemy>
{
    public override void CaptureInput()
    {
        
    }

    public override void CheckStateTransition()
    {
        if (Runner.GetDetectCheck().Check()){
            List<GameObject> objsDetected =  Runner.GetDetectCheck().GetObjectsInCheck();
            
            Runner.SetMainState(typeof(EnemyWalkTowardsState), objsDetected[0].gameObject);
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void InitialiseSubState()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void UpdateState()
    {
    }

    public override void OnStateCollisionStay(Collision2D collision)
    {
    }

    public override void OnStateCollisionExit(Collision2D collision)
    {
    }
}