using System;
using System.Collections.Generic;
using UnityEngine;

// From ZX Breathe Easy

public enum StateTransitionPhase
{
    Entered,
    TransitionSuspected,
    Exited
}

/** Manages a list of states and the transitions between them.

    Inputs: 
        EnumType: needs to be an enum that holds one label for each state
        ValueType: the type of the input to the state machine

    You define these two functions for each state:

        bool IsUndergoingTransition(ValueType); - determines when a transition takes place
        EnumType GetNextState(ValueType);       - returns the next state after the transition

    Each state's transition has a minimum duration that can be set for each individual state.
    The default is 0.1 seconds, but this can be set to zero for instant transitions. If it is not
    zero, the boolean returned by IsUndergoingTransition must remain true for the entire duration
    before a transition will take place.

    There is an optional callback that happens immediately when IsUndergoingTransition first returns
    true, so you can capture the value at that point. This is useful for things like grabbing a peak
    value when the input goes from increasing to decreasing.
    */
public class AbstractStateMachine<EnumType, ValueType> where EnumType : struct, IConvertible
{
    private readonly Dictionary<EnumType, State> m_states;
    private EnumType m_currentType;
    private State m_currentState;
    private float m_timer;
    public Action<EnumType> StateChangeCallback;
    private bool m_first = true;

    public AbstractStateMachine(Dictionary<EnumType, State> allStates, EnumType initialState, Action<EnumType> stateChangeCallback = null)
    {
        m_states = allStates;
        m_currentType = initialState;
        m_currentState = m_states[m_currentType];
        m_timer = m_currentState.TransitionDuration;
        StateChangeCallback = stateChangeCallback;
        m_first = true;
    }

    public void GoToState(EnumType state)
    {
        m_currentType = state;
        m_currentState.TransitionCallback?.Invoke(StateTransitionPhase.Exited);
        StateChangeCallback?.Invoke(m_currentType);
        m_currentState = m_states[m_currentType];
        m_currentState.TransitionCallback?.Invoke(StateTransitionPhase.Entered);
    }

    public EnumType GetCurrentState()
    {
        return m_currentType;
    }

    public EnumType UpdateWithValue(ValueType newValue)
    {
        if (m_first == true)
        {
            m_first = false;
            StateChangeCallback?.Invoke(m_currentType);
            m_currentState.TransitionCallback?.Invoke(StateTransitionPhase.Entered);
        }
        m_currentState.HandleNewValue?.Invoke(newValue);
        if (m_currentState.IsUndergoingTransition?.Invoke(newValue) ?? false)
        {
            if (EqualityComparer<float>.Default.Equals(m_timer, m_currentState.TransitionDuration))
            {
                m_currentState.TransitionCallback?.Invoke(StateTransitionPhase.TransitionSuspected);
            }
            m_timer -= Time.deltaTime;
            if (m_timer <= 0)
            {
                GoToState(m_currentState.NextState?.Invoke(newValue) ?? default);
            }
        }
        else
        {
            m_timer = m_currentState.TransitionDuration;
        }
        return m_currentType;
    }

    public class State
    {
        public State(IHandler handler) { m_handler = handler; }

        public interface IHandler
        {
            Func<ValueType, EnumType> GetNextState { get; }
            Func<ValueType, bool> IsTransitionHappening { get; }
            Action<StateTransitionPhase> TransitionCallback { get; }
            Action<ValueType> HandleValueCallback { get; }
            float TransitionDuration { get; }
        }

        public struct Handler : IHandler
        {
            public Func<ValueType, EnumType> GetNextState { get; set; }
            public Func<ValueType, bool> IsTransitionHappening { get; set; }
            public Action<StateTransitionPhase> TransitionCallback { get; set; }
            public Action<ValueType> HandleValueCallback { get; set; }
            public float TransitionDuration { get; set; }
        }
        private IHandler m_handler;

        public Func<ValueType, EnumType> NextState { get { return m_handler.GetNextState; } }
        public Func<ValueType, bool> IsUndergoingTransition { get { return m_handler.IsTransitionHappening; } }
        public Action<StateTransitionPhase> TransitionCallback { get { return m_handler.TransitionCallback; } }
        public const float DefaultTransitionDuration = 0.1f;
        public float TransitionDuration { get { return m_handler.TransitionDuration; } }
        public Action<ValueType> HandleNewValue
        {
            get { return m_handler.HandleValueCallback; }
        }
    }
}
