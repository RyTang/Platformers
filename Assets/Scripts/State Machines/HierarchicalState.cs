using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class HierarchicalState<T> : State<T> where T: MonoBehaviour
{
    [SerializeField] protected List<State<T>> _states;
    private State<T> active_state;

    protected virtual void Awake(){}

    protected virtual void Start(){
    }

    public virtual void SetSubState(Type newStateType, float floatVariable){
        SetSubState(newStateType);
    }

    public virtual void SetSubState(Type newStateType){
        if (active_state != null){
            active_state.ExitState();
        }
        active_state = _states.First(s => s.GetType() == newStateType);
        active_state.EnterState(_runner.GetComponent<T>());
    }

    /// <summary>
    /// Handles the SubStates that needs to be changed
    /// </summary>

    public override void Update()
    {
        active_state.CaptureInput();
        active_state.Update();
        active_state.ChangeState();
    }

    public override void FixedUpdate()
    {
        active_state.FixedUpdate();
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
        active_state.OnStateCollisionEnter(collision);
    }
}