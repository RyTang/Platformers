using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStateRunner<T> : MonoBehaviour where T: MonoBehaviour
{
    [SerializeField] private List<State<T>> _states;
    private State<T> active_state;

    protected virtual void Awake(){}
    
    protected virtual void Start(){
        SetState(_states[0].GetType());
    }


    public void SetState(Type newStateType){
        if (active_state != null){
            active_state.ExitState();
        }
        active_state = _states.First(s => s.GetType() == newStateType);
        active_state.EnterState(GetComponent<T>());
    }

    public virtual void Update(){
        active_state.CaptureInput();
        active_state.Update();
        active_state.ChangeState();
    }

    public virtual void FixedUpdate()
    {
        active_state.FixedUpdate();
    }

    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        active_state.OnStateCollisionEnter(other);
    }
}