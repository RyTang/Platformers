using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Player State/Speed State/Main State")]
public class SpeedMainState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, sprintControl;

    private Rigidbody2D rb2d;

    private bool sprintMode = true;

    private Coroutine energyConsumption;
    private Coroutine energyRecovery;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);
        sprintMode = true;

        if (energyRecovery != null ){
            Runner.StopCoroutine(energyRecovery);
            energyRecovery = null;
        }

        rb2d = parent.GetRigidbody2D();   
        energyConsumption = Runner.StartCoroutine(EnergyConsumption());
        InitialiseSubState();
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();
        sprintControl = Runner.GetSprintControls();

        if (sprintControl > 0 && Runner.GetPlayerData().currentEnergy > 0){
            sprintMode = true;
        } 
        else{
            sprintMode = false;
        }
    }

    public override void CheckStateTransition()
    {
        if (!sprintMode){
            Runner.SetMainState(typeof(NormalMainState));
        }
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
        while (Runner.GetPlayerData().currentEnergy > 0){
            Runner.GetPlayerData().currentEnergy -= Runner.GetPlayerData().sprintDepletionRate;
            Debug.Log(Runner.GetPlayerData().currentEnergy);

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
        else if (verticalControl > 0) {
            SetSubState(Runner.GetState(typeof(SpeedJumpState)));
        }
        else if (!Runner.GetGroundCheck().Check() || (Runner.GetRigidbody2D().velocity.y < 0)){
            SetSubState(Runner.GetState(typeof(SpeedFallState)));
        }
        else {
            SetSubState(Runner.GetState(typeof(SpeedIdleState)));
        }
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
