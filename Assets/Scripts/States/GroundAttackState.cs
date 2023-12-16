using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Ground Attack")]
public class GroundAttackState : State<PlayerController>
{
    private bool attacking;
    private Coroutine attackDelay;

    public override void EnterState(PlayerController parent)
    {

        base.EnterState(parent);
        attacking = true;
        Debug.Log("Attacking");


        // Prevent from Moving  
        _runner.GetRigidbody2D().velocity = new Vector2(0, _runner.GetRigidbody2D().velocity.y);

        if (attackDelay != null)
        {
            _runner.StopCoroutine(attackDelay);
            attackDelay = null;
        }

        Attack();

        if (attacking) attackDelay = _runner.StartCoroutine(AttackDelay());
    }

    private void Attack()
    {
        List<GameObject> attackedObjects = _runner.GetAttackCheck().GetObjectsInCheck();

        foreach (GameObject attackedObject in attackedObjects)
        {
            IDamageable damageable = attackedObject.GetComponent<IDamageable>();
            damageable.TakeDamage(_runner.GetPlayerData().attackDamage);
        }
    }

    private IEnumerator AttackDelay(){
        yield return new WaitForSeconds(_runner.GetPlayerData().attackTime);
        attacking = false;
    }

    public override void CaptureInput()
    {
    }

    public override void ChangeState()
    {
        if (!attacking){
            _runner.SetState(typeof(WalkState));
        }
    }

    public override void ExitState()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void Update()
    {
    }
}