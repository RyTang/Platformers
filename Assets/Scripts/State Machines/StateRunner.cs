using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateRunner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private List<BaseState<T>> _states;
    [SerializeField] Dictionary<Type, BaseState<T>> _cacheStates = new Dictionary<Type, BaseState<T>>();
    private BaseState<T> active_state;

    public Dictionary<Type, BaseState<T>> CacheStates { get => _cacheStates; set => _cacheStates = value; }

    protected virtual void Awake() { }

    protected virtual void Start()
    {
        SetMainState(_states[0].GetType());
    }

    public BaseState<T> GetState(Type stateTypeWanted)
    {
        if (CacheStates.ContainsKey(stateTypeWanted)){
            return CacheStates[stateTypeWanted];
        }
        else{
            BaseState<T> newCacheState = Instantiate(_states.First(s => s.GetType() == stateTypeWanted));
            CacheStates.Add(stateTypeWanted, newCacheState);
            return newCacheState;
        }
    }

    /// <summary>
    /// Sets the Next Main State
    /// </summary>
    /// <param name="newStateType">Type of State that should be added into</param>
    public void SetMainState(Type newStateType)
    {
        StartCoroutine(CleanStates(newStateType, null));
    }

    /// <summary>
    /// Sets the Next Main State while allowing to pass through an object
    /// </summary>
    /// <param name="newStateType">Type of State that should be added into</param>
    /// <param name="objToPass">Object to pass to the next State</param>
    public void SetMainState(Type newStateType, object objToPass){
        StartCoroutine(CleanStates(newStateType, objToPass));
    }

    private IEnumerator CleanStates(Type newStateType, object parameter)
    {
        if (active_state != null)
        {
            yield return StartCoroutine(active_state.ExitStates());
        }

        active_state = GetState(newStateType);
        Debug.Assert(active_state != null, gameObject + ": UNABLE TO FIND STATE " + newStateType);

        if (parameter is float)
        {
            active_state.EnterState(GetComponent<T>(), (float) parameter);
        }
        else if (parameter is GameObject)
        {
            active_state.EnterState(GetComponent<T>(), (GameObject) parameter);
        }
        else{
            active_state.EnterState(GetComponent<T>());
        }

    }

    public virtual void Update()
    {
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
