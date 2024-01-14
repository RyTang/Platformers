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

    protected BaseState<T> _currentSubState;
    protected BaseState<T> _currentSuperState;


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
    /// <param name="floatToPass">Data that should be passed</param>
    public virtual void EnterState(T parent, float floatToPass){
        EnterState(parent);
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
    /// Handles Collision Interaction
    /// </summary>
    /// <param name="collision"></param>
    public abstract void OnStateCollisionEnter(Collision2D collision);
    
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
        if (_currentSubState != null){
            _currentSubState.UpdateStates();
        }
    }

    /// <summary>
    /// Recursively Fixed Updates the Sub States
    /// </summary>
    public void FixedUpdateStates(){
        FixedUpdateState();
        if (_currentSubState != null){
            _currentSubState.FixedUpdateStates();
        }
    }

    /// <summary>
    /// Recursively Exits the Current State and Sub States
    /// </summary>
    public virtual IEnumerator ExitStates(){
        IsStateActive = false;
        if (_currentSubState != null){
            yield return Runner.StartCoroutine(_currentSubState.ExitStates());
        }
        yield return Runner.StartCoroutine(ExitState());
    }


    /// <summary>
    /// Sets the Parent State of this State
    /// </summary>
    /// <param name="newSuperState">Parent State</param>
    protected void SetSuperState(BaseState<T> newSuperState){
        _currentSuperState = newSuperState;
    }

    /// <summary>
    /// Child Sub State attached to this State
    /// </summary>
    /// <param name="newSubState">Sub State</param>
    public void SetSubState(BaseState<T> newSubState){
        if (_currentSubState != null) _currentSubState.ExitStates();
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
        _currentSubState.EnterState(Runner);
    }
}