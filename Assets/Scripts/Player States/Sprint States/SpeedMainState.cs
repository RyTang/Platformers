using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Player State/Speed State/Main State")]
public class SpeedMainState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, attackControl, sprintControl;

    private Rigidbody2D rb2d;

    private bool sprintMode = true;

    private bool canJump = false;
    private Coroutine coyoteTimer;
    private Coroutine energyConsumption;
    private Coroutine energyRecovery;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        rb2d = parent.GetRigidbody2D();   
        energyConsumption = Runner.StartCoroutine(EnergyConsumption());
        InitialiseSubState();
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        attackControl = Runner.GetAttackControls();
        sprintControl = Runner.GetSprintControls();

        if (sprintControl > 0){
            sprintMode = true;
        } 
        else if (sprintControl <= 0) {
            sprintMode = false;
        }
    }

    public override void CheckStateTransition()
    {
        if (!sprintMode){
            Runner.SetMainState(typeof(NormalMainState));
        }

        // if (!sprintMode && horizontalControl != 0){
        //     Runner.SetMainState(typeof(SpeedRunState));
        // }
        // else if (attackControl > 0) {
        //     Runner.SetMainState(typeof(GroundSubAttackOne));
        // }
        // else if (horizontalControl == 0){
        //     Runner.SetMainState(typeof(SpeedIdleState));
        // }
        // else if (verticalControl > 0 && canJump) {
        //     Runner.SetMainState(typeof(SpeedJumpState), Runner.GetRigidbody2D().velocity.x);
        // }
        // else if ((verticalControl < 0 && !Runner.GetGroundCheck().Check()) || (Runner.GetRigidbody2D().velocity.y < 0 && !canJump)){
        //     Runner.SetMainState(typeof(SpeedFallState));
        // }
        // else if (horizontalControl != 0 && Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check()){
        //     Runner.SetMainState(typeof(SpeedWallClingState));
        // }
    }

    public override void FixedUpdateState()
    {
    }

    public override IEnumerator ExitState(){
        // FIXME: BUG when energy recovery may happen during consumption
        sprintMode = false;
        if (energyConsumption != null) {
            Runner.StopCoroutine(energyConsumption);
            energyConsumption = null;
        }
        energyRecovery ??= Runner.StartCoroutine(EnergyRecovery());
        yield break;
    }

    private IEnumerator EnergyRecovery(){
        while (Runner.GetPlayerData().currentEnergy < Runner.GetPlayerData().maxEnergyBar){
            Runner.GetPlayerData().currentEnergy += Runner.GetPlayerData().energyRecoveryRate;
            Runner.GetPlayerData().currentEnergy = Mathf.Min(Runner.GetPlayerData().currentEnergy, Runner.GetPlayerData().maxEnergyBar);
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator EnergyConsumption(){
        while (sprintMode){
            Runner.GetPlayerData().currentEnergy -= Runner.GetPlayerData().sprintDepletionRate;

            if (Runner.GetPlayerData().currentEnergy <= 0){
                sprintMode = false;
            }

            yield return new WaitForSeconds(1);
        }
    }

    public override void InitialiseSubState()
    {
        CaptureInput();
        
        if (!sprintMode) return;
        
        if (horizontalControl != 0){
            if (Runner.GetWallCheck().Check() && !Runner.GetGroundCheck().Check()){
                SetSubState(Runner.GetState(typeof(SpeedWallClingState)));
            }
            else {
                SetSubState(Runner.GetState(typeof(SpeedRunState)));
            }
        }
        else if (horizontalControl == 0){
            SetSubState(Runner.GetState(typeof(SpeedIdleState)));
        }
        else if (attackControl > 0) {
            Runner.SetMainState(typeof(GroundSubAttackOne));
        }
        else if (verticalControl > 0 && canJump) {
            SetSubState(Runner.GetState(typeof(SpeedJumpState)));
        }
        else if ((verticalControl < 0 && !Runner.GetGroundCheck().Check()) || (Runner.GetRigidbody2D().velocity.y < 0 && !canJump)){
            SetSubState(Runner.GetState(typeof(SpeedFallState)));
        }
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void UpdateState()
    {
    }
}
