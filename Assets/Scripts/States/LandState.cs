using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Land")]
public class LandState : State<PlayerController>
{

    private bool canMove;

    public override void EnterState(PlayerController parent, float floatVariable)
    {
        base.EnterState(parent, floatVariable);

        canMove = !(floatVariable >= _runner.GetPlayerData().landVelocityThreshold);
        LandCheck(canMove);
    }

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        // FIXME: Velocity is 0 due to when entering the state, it has already touched the ground

        Debug.Log("Landing Velocity: " + _runner.GetRigidbody2D().velocity.y);

        canMove = !(Mathf.Abs(_runner.GetRigidbody2D().velocity.y) >= _runner.GetPlayerData().landVelocityThreshold);
        LandCheck(canMove);
    }

    private void LandCheck(bool canMove)
    {
        if (!canMove)
        {
            // TODO: Create Animation here
            _runner.StopCoroutine(LandDelay());
            _runner.StartCoroutine(LandDelay());
        }

        _runner.GetAnimator().SetBool(PlayerAnimation.isLandingBool, true);
    }

    private IEnumerator LandDelay(){
        yield return new WaitForSeconds(_runner.GetPlayerData().landDelay);
        canMove = true;
    }

    public override void CaptureInput()
    {
    }
 
    public override void ChangeState()
    {
        if (canMove) {
            _runner.SetState(typeof(IdleState));
        }
    }

    public override void ExitState()
    {
        _runner.GetAnimator().SetBool(PlayerAnimation.isLandingBool, false);
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