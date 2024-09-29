using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseState<T> : ScriptableObject where T : MonoBehaviour
{
    protected T Runner { get; set; }
    protected bool IsStateActive { get; set; } = true;
    protected BaseState<T> CurrentSubState { get; set; }
    protected BaseState<T> CurrentSuperState { get; set; }
    [SerializeField] protected List<BaseState<T>> availableSubStates = new List<BaseState<T>>();
    protected Dictionary<Type, BaseState<T>> cachedSubStates = new Dictionary<Type, BaseState<T>>();
    private bool switchingState = false;

    public bool GetIsRootState(){
        return availableSubStates.Count <= 0;
    }
    
    public BaseState<T> GetSubState(){
        return CurrentSubState;
    }

    public BaseState<T> GetFinalLeafState(){
        BaseState<T> currentState = this;
        int counter = 0;
        Debug.Log($"Checking State : {currentState}");
        while (!currentState.GetIsRootState() && counter <= 10){
            currentState = currentState.GetSubState();
            Debug.Log($"Checking State : {currentState}");
            counter++;
        }

        return currentState;
    }

    public BaseState<T> GetState(Type stateTypeWanted){
        if (cachedSubStates.ContainsKey(stateTypeWanted)){
            return cachedSubStates[stateTypeWanted];
        }
        else {
            BaseState<T> newCacheState = null;
            try {
                newCacheState = Instantiate(availableSubStates.First(s => s.GetType() == stateTypeWanted));

                cachedSubStates.Add(stateTypeWanted, newCacheState);
            }
            catch {
                Debug.LogError("Unable to find state Wanted: " + stateTypeWanted.Name + " in object: " + this);
                // Default to first state if unable to find state but flag error
                newCacheState = cachedSubStates.FirstOrDefault().Value;
            }
            return newCacheState;
        }
    }
    

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
    public virtual void CaptureInput(){}
    
    /// <summary>
    /// Code that should be run in Update
    /// </summary>
    public virtual void UpdateState(){}
    
    /// <summary>
    /// Code that runs in Fixed Update
    /// </summary>
    public virtual void FixedUpdateState(){}


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
    public virtual void OnStateCollisionEnter(Collision2D collision){}

    /// <summary>
    /// Handles Collision Stay Interaction
    /// </summary>
    /// <param name="collision"></param>
    public virtual void OnStateCollisionStay(Collision2D collision){}

    /// <summary>
    /// Handles Collision Exit Interaction
    /// </summary>
    /// <param name="collision"></param>
    public virtual void OnStateCollisionExit(Collision2D collision){}

    
    /// <summary>
    /// Checks the Sub State taht should be defaulted transitioned into
    /// </summary>
    public virtual void InitialiseSubState(){} 

    /// <summary>
    /// Recursively Updates the Sub States
    /// </summary>
    public void UpdateStates(){
        if (!IsStateActive) return;

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
            CurrentSubState.OnStatesCollisionEnter(collision);
        }
    }

    /// <summary>
    /// Handles Collision Interaction Stay for all states within this state
    /// </summary>
    /// <param name="collision"></param>
    public void OnStatesCollisionStay(Collision2D collision){
        OnStateCollisionStay(collision);
        if (CurrentSubState != null){
            CurrentSubState.OnStatesCollisionStay(collision);
        }
    }

    /// <summary>
    /// Handles Collision Interaction for all states within this state
    /// </summary>
    /// <param name="collision"></param>
    public void OnStatesCollisionExit(Collision2D collision){
        OnStateCollisionExit(collision);
        if (CurrentSubState != null){
            CurrentSubState.OnStatesCollisionExit(collision);
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
        if (switchingState) return;
        Runner.StartCoroutine(CleanSubStates(newSubState, null));
    }

    /// <summary>
    /// Child Sub State attached to this State
    /// </summary>
    /// <param name="newSubState">Sub State</param>
    public void SetSubState(BaseState<T> newSubState, object objToPass){
        if (switchingState) return;
        Runner.StartCoroutine(CleanSubStates(newSubState, objToPass));
    }

    public IEnumerator CleanSubStates(BaseState<T> newSubState, object objToPass){
        switchingState = true;
        if (CurrentSubState != null){
            yield return Runner.StartCoroutine(CurrentSubState.ExitStates());
        }
        Debug.Assert(newSubState != null, $"Experienced Error in Clean Sub States, missing Sub State: {newSubState}");
        CurrentSubState = newSubState;
        newSubState.SetSuperState(this);

        if (objToPass != null){
            CurrentSubState.EnterState(Runner, objToPass);
        }
        else{
            CurrentSubState.EnterState(Runner);
        }
        switchingState = false;
    }
}