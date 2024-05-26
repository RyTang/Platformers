using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy State/Enemy Fly Chase")]
public class EnemyFlyTowardsState : EnemyMoveState
{
    private GameObject objToChase;

    private bool isChasing;

    private float timer;

    public override void EnterState(BaseEnemy parent, object objToPass)
    {
        Debug.Log("Entering Enemy Walk State ");

        objToChase = (GameObject) objToPass;
        isChasing = true;
        base.EnterState(parent, objToPass);
    }

    public override void CaptureInput()
    {
    }

    public override void CheckStateTransition()
    {
        if (!isChasing){
            Runner.SetMainState(typeof(EnemyDetectTargetState));
        }
        else if (Runner.GetAttackCheck().Check()){
            // TODO: Attempt to Attack
            Runner.SetMainState(typeof(EnemyAttackState));
        }
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

    public override void FixedUpdateState()
    {
        Vector2 targetDirection = (objToChase.transform.position - Runner.transform.position).normalized;
        
        Runner.GetRigidbody2D().velocity = targetDirection * Runner.GetBasicEnemydata().moveSpeed;
        // TODO: Consider if Jumping is an option;
    }

    public override void InitialiseSubState()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void OnStateCollisionStay(Collision2D collision)
    {
    }

    public override void OnStateCollisionExit(Collision2D collision)
    {
    }
}