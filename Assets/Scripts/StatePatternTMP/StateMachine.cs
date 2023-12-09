using System.Collections.Generic;

namespace Gabevlogd.Patterns
{

    /// <summary>
    /// State manager to handle basic states
    /// </summary>
    /// <typeparam name="TContext">The type of the context</typeparam>
    public class StateMachine<TContext> 
    {
        public TContext Context;
        public StateBase<TContext> CurrentState;
        public StateBase<TContext> PreviousState;
        public List<StateBase<TContext>> StateList;

        public StateMachine(TContext context)
        {
            Context = context;
        }

        public void RunStateMachine(StateBase<TContext> entryPoint)
        {
            StateBase<TContext>.OnChangeState += ChangeState;
            CurrentState = entryPoint;
            CurrentState.OnEnter(Context);
        }

        public void AddState(StateBase<TContext> state)
        {
            StateList = (StateList == null) ? new List<StateBase<TContext>>() : StateList;
            StateList.Add(state);
        }

        /// <summary>
        /// Changes the current state to the passed state (only if the current state is not already the passed state)
        /// </summary>
        public void ChangeState(StateBase<TContext> state)
        {
            if (CurrentState == state) return;

            PreviousState = CurrentState;
            CurrentState.OnExit(Context);
            CurrentState = state;
            CurrentState.OnEnter(Context);
        }
    }
}


