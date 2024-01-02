using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class State<T> : ScriptableObject where T: MonoBehaviour
{
    protected T _runner;
    
    public virtual void EnterState(T parent){
        _runner = parent;
    }

    public virtual void EnterState(T parent, float floatVariable){
        EnterState(parent);
    }

    /// <summary>
    /// What Inputs to look and control
    /// </summary>
    public abstract void CaptureInput();
    
    /// <summary>
    /// Code that should be run in Update
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Code that runs in Fixed Update
    /// </summary>
    public abstract void FixedUpdate();

    /// <summary>
    /// Code that is run on Update to check if a transition needs to be changed
    /// </summary>
    public abstract void ChangeState();

    /// <summary>
    /// Exit State is called when transitioning to the next State. Clean Up should happen here
    /// </summary>
    public abstract void ExitState();

    /// <summary>
    /// Handles Collision Interaction
    /// </summary>
    /// <param name="collision"></param>
    public abstract void OnStateCollisionEnter(Collision2D collision);
}