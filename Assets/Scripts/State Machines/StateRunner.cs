using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateRunner<T>: MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private List<BaseState<T>> _states;
    private BaseState<T> active_state;

    protected virtual void Awake(){}
    
    protected virtual void Start(){
        SetMainState(_states[0].GetType());
    }

    public BaseState<T> GetState(Type stateTypeWanted){
        return _states.First(s => s.GetType() == stateTypeWanted);
    }

    /// <summary>
    /// Changes the current state to a new State
    /// </summary>
    /// <param name="newStateType">The new State type to be switched into</param>
    public void SetMainState(Type newStateType){

        if (active_state != null){
            active_state.ExitStates();
        }
        active_state = _states.First(s => s.GetType() == newStateType);
        active_state.EnterState(GetComponent<T>());
    }

    public void SetMainState(Type newStateType, float floatVariable){
        if (active_state != null){
            active_state.ExitStates();
        }
        active_state = _states.First(s => s.GetType() == newStateType);
        active_state.EnterState(GetComponent<T>(), floatVariable);
    }    

    public virtual void Update(){
        active_state.UpdateStates();
    }

    public virtual void FixedUpdate()
    {
        active_state.FixedUpdateStates();
    }

    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        active_state.OnStateCollisionEnter(other);
    }
}