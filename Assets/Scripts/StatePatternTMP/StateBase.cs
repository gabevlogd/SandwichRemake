using System;
using UnityEngine;

namespace Gabevlogd.Patterns
{

    /// <summary>
    /// Basic state template 
    /// </summary>
    /// <typeparam name="TContext">The type of the context</typeparam>
    public class StateBase<TContext>
    {
        public string StateID;
        public static event Action<StateBase<TContext>> OnChangeState;

        public StateBase(string stateID)
        {
            StateID = stateID;
        }

        public virtual void OnEnter(TContext context)
        {
            Debug.Log("OnEnter " + StateID);

        }

        public virtual void OnUpdate(TContext context)
        {
            Debug.Log("OnUpadte " + StateID);
        }

        public virtual void OnExit(TContext context)
        {
            Debug.Log("OnExit " + StateID);
        }

        protected void ChangeState(StateBase<TContext> state) => OnChangeState?.Invoke(state);
    }
}

