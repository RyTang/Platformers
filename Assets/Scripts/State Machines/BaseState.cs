using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseState<T> : ScriptableObject where T : MonoBehaviour
{
    private T _runner;
    protected T Runner { get => _runner; set => _runner = value; }
    protected bool IsStateActive { get => isStateActive; set => isStateActive = value; }
    protected bool IsRootState { get => isRootState; set => isRootState = value; }
    protected BaseState<T> CurrentSubState { get => currentSubState; set => currentSubState = value; }
    protected BaseState<T> CurrentSuperState { get => currentSuperState; set => currentSuperState = value; }

    private BaseState<T> currentSubState;
    private BaseState<T> currentSuperState;


    private bool isStateActive = true;
    private bool isRootState = false;

    /// <summary>
    /// Enters state while setting Parent State Runner running the script
    /// </summary>
    /// <param name="parent">Parent State Runner running the script</param>
    public virtual void EnterState(T parent){
        Runner = parent;
        IsStateActive = true;
    }

    /// <summary>
    /// Enters State while passing information regarding any variable that the State Requires
    /// </summary>
    /// <param name="parent">Parent State Runner that is running the script</param>
    /// <param name="objToPass">Data that should be passed</param>
    public virtual void EnterState(T parent, object objToPass){
        EnterState(parent);
    }

    /// <summary>
    /// What Inputs to look and control
    /// </summary>
    public abstract void CaptureInput();
    
    /// <summary>
    /// Code that should be run in Update
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// Code that runs in Fixed Update
    /// </summary>
    public abstract void FixedUpdateState();


    /// <summary>
    /// Code that is run on Update to check if a transition needs to be changed
    /// </summary>
    public abstract void CheckStateTransition();


    /// <summary>
    /// Exit State is called when transitioning to the next State. Clean Up should happen here
    /// </summary>
    public virtual IEnumerator ExitState(){yield break;}

    /// <summary>
    /// Handles Collision Enter Interaction
    /// </summary>
    /// <param name="collision"></param>
    public abstract void OnStateCollisionEnter(Collision2D collision);

    /// <summary>
    /// Handles Collision Stay Interaction
    /// </summary>
    /// <param name="collision"></param>
    public abstract void OnStateCollisionStay(Collision2D collision);

    /// <summary>
    /// Handles Collision Exit Interaction
    /// </summary>
    /// <param name="collision"></param>
    public abstract void OnStateCollisionExit(Collision2D collision);

    
    /// <summary>
    /// Checks the Sub State taht should be defaulted transitioned into
    /// </summary>
    public abstract void InitialiseSubState(); 

    /// <summary>
    /// Recursively Updates the Sub States
    /// </summary>
    public void UpdateStates(){
        if (!isStateActive) return;

        CaptureInput();
        CheckStateTransition();
        UpdateState();
        if (CurrentSubState != null){
            CurrentSubState.UpdateStates();
        }
    }

    /// <summary>
    /// Recursively Fixed Updates the Sub States
    /// </summary>
    public void FixedUpdateStates(){
        FixedUpdateState();
        if (CurrentSubState != null){
            CurrentSubState.FixedUpdateStates();
        }
    }

    /// <summary>
    /// Recursively Exits the Current State and Sub States
    /// </summary>
    public virtual IEnumerator ExitStates(){
        IsStateActive = false;
        if (CurrentSubState != null){
            yield return Runner.StartCoroutine(CurrentSubState.ExitStates());
        }
        yield return Runner.StartCoroutine(ExitState());
    }

    /// <summary>
    /// Handles Collision Interaction for all states within this state
    /// </summary>
    /// <param name="collision"></param>
    public void OnStatesCollisionEnter(Collision2D collision){
        OnStateCollisionEnter(collision);
        if (CurrentSubState != null){
            currentSubState.OnStatesCollisionEnter(collision);
        }
    }

    /// <summary>
    /// Handles Collision Interaction Stay for all states within this state
    /// </summary>
    /// <param name="collision"></param>
    public void OnStatesCollisionStay(Collision2D collision){
        OnStateCollisionStay(collision);
        if (CurrentSubState != null){
            currentSubState.OnStatesCollisionStay(collision);
        }
    }

    /// <summary>
    /// Handles Collision Interaction for all states within this state
    /// </summary>
    /// <param name="collision"></param>
    public void OnStatesCollisionExit(Collision2D collision){
        OnStateCollisionExit(collision);
        if (CurrentSubState != null){
            currentSubState.OnStatesCollisionExit(collision);
        }
    }


    /// <summary>
    /// Sets the Parent State of this State
    /// </summary>
    /// <param name="newSuperState">Parent State</param>
    protected void SetSuperState(BaseState<T> newSuperState){
        CurrentSuperState = newSuperState;
    }

    /// <summary>
    /// Child Sub State attached to this State
    /// </summary>
    /// <param name="newSubState">Sub State</param>
    public void SetSubState(BaseState<T> newSubState){
        Runner.StartCoroutine(CleanSubStates(newSubState, null));
    }

    /// <summary>
    /// Child Sub State attached to this State
    /// </summary>
    /// <param name="newSubState">Sub State</param>
    public void SetSubState(BaseState<T> newSubState, object objToPass){
        Runner.StartCoroutine(CleanSubStates(newSubState, objToPass));
    }

    public IEnumerator CleanSubStates(BaseState<T> newSubState, object objToPass){
        if (currentSubState != null){
            yield return Runner.StartCoroutine(CurrentSubState.ExitStates());
        }
        CurrentSubState = newSubState;
        newSubState.SetSuperState(this);

        if (objToPass != null){
            CurrentSubState.EnterState(Runner, objToPass);
        }
        else{
            CurrentSubState.EnterState(Runner);
        }
    }
}