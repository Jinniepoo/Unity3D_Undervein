using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diablo.AI
{
    [Serializable]
    public abstract class State<T>
    {
        protected int mecanimStateHash;
        protected StateMachine<T> stateMachine;
        protected T context;

        public State()
        {
        }

        public State(string mecanimStateName) : this(Animator.StringToHash(mecanimStateName))
        {
        }

        public State(int mecanimStateHash)
        {
            this.mecanimStateHash = mecanimStateHash;
        }

        internal void SetMachineAndContext(StateMachine<T> stateMachine, T context)
        {
            this.stateMachine = stateMachine;
            this.context = context;

            OnInitialized();
        }

        public virtual void OnInitialized()
        { }

        public virtual void OnEnter()
        { }

        public virtual void PreUpdate()
        { }

        public abstract void Update(float deltaTime);

        public virtual void OnExit()
        { }
    }

    public sealed class StateMachine<T>
    {
        private T context;
        public event Action OnChangedState;

        private State<T> currentState;
        public State<T> CurrentState => currentState;

        private State<T> previousState;
        public State<T> PreviousState => previousState;

        private float elapsedTimeInState = 0.0f;
        public float ElapsedTimeInState => elapsedTimeInState;

        private Dictionary<System.Type, State<T>> states = new Dictionary<Type, State<T>>();

        public StateMachine(T context, State<T> initialState)
        {
            this.context = context;

            AddState(initialState);
            currentState = initialState;
            currentState.OnEnter();
        }
        public void AddState(State<T> state)
        {
            state.SetMachineAndContext(this, context);
            states[state.GetType()] = state;
        }

        public void Update(float deltaTime)
        {
            elapsedTimeInState += deltaTime;

            currentState.PreUpdate();
            currentState.Update(deltaTime);
        }

        public R ChangeState<R>() where R : State<T>
        {
            var newType = typeof(R);
            if (currentState.GetType() == newType)
            {
                return currentState as R;
            }

            if (currentState != null)
            {
                currentState.OnExit();
            }


#if UNITY_EDITOR
            if (!states.ContainsKey(newType))
            {
                var error = GetType() + ": state " + newType + " does not exist. Did you forget to add it by calling addState?";
                Debug.LogError("error");
                throw new Exception(error);
            }
#endif

            previousState = currentState;
            currentState = states[newType];
            currentState.OnEnter();
            elapsedTimeInState = 0.0f;

            if (OnChangedState != null)
            {
                OnChangedState();
            }

            return currentState as R;
        }

    }
}
