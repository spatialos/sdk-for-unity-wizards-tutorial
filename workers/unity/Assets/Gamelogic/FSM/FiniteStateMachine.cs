using System.Collections.Generic;
using UnityEngine;

namespace Assets.Gamelogic.FSM
{
    public abstract class FiniteStateMachine<TStateEnum>
    {
        public TStateEnum CurrentState { get; private set; }

        private IDictionary<TStateEnum, IList<TStateEnum>> transitions;
        private IDictionary<TStateEnum, IFsmState> states;

        protected void SetStates(IDictionary<TStateEnum, IFsmState> inStates)
        {
            states = inStates;
        }

        protected void SetTransitions(IDictionary<TStateEnum, IList<TStateEnum>> inTransitions)
        {
            transitions = inTransitions;
        }

        public void Tick()
        {
            states[CurrentState].Tick();
        }

        public void OnEnable(TStateEnum initialState)
        {
            OnEnableImpl();
            CurrentState = initialState;
            states[CurrentState].Enter();          
        }

        public void OnDisable()
        {
            states[CurrentState].Exit(true);
            OnDisableImpl();
        }

        protected virtual void OnEnableImpl() { }

        protected virtual void OnDisableImpl() { }

        public void TransitionTo(TStateEnum nextState)
        {
            if (IsValidTransition(nextState))
            {
                states[CurrentState].Exit(false);
                CurrentState = nextState;
                states[CurrentState].Enter();
            }
            else
            {
                Debug.LogErrorFormat("Invalid transition from {0} to {1} detected.", CurrentState, nextState);
            }
        }

        public bool IsValidTransition(TStateEnum nextState)
        {
            return transitions[CurrentState].Contains(nextState);
        }
    }
}
