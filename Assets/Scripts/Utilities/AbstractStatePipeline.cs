using UnityEngine;
using System;
using System.Collections.Generic;

// From ZX Breathe Easy
public class AbstractStatePipeline<EnumType> where EnumType : struct, IConvertible
{
    // PipelineElements are used to build up a pipeline.
    //================================================================================
    public abstract class PipelineState : AbstractStateMachine<EnumType, AbstractStatePipeline<EnumType>>.State.IHandler
    {
        // Must Be Overridden
        //============================================================================
        public abstract EnumType NextState(AbstractStatePipeline<EnumType> pipeline);
        public abstract bool ShouldTransition(AbstractStatePipeline<EnumType> pipeline);

        // Optional Methods
        //============================================================================
        public virtual void HandleNewValue(AbstractStatePipeline<EnumType> pipeline) { }
        public virtual void HandleTransition(StateTransitionPhase transitionPhase) { }
        public virtual void Reset(AbstractStatePipeline<EnumType> pipeline) { }

        // AbstractStateMachine.State.IHandler
        //============================================================================
        public Func<AbstractStatePipeline<EnumType>, EnumType> GetNextState { get { return NextState; } }
        public Func<AbstractStatePipeline<EnumType>, bool> IsTransitionHappening { get { return ShouldTransition; } }
        public Action<AbstractStatePipeline<EnumType>> HandleValueCallback { get { return HandleNewValue; } }
        public float TransitionDuration { get { return 0; } }
        public Action<StateTransitionPhase> TransitionCallback { get { return HandleTransition; } }
    }

    // Convert a list of PipelineElements into a list of AbstractStateMachine.States
    //================================================================================
    private static Dictionary<EnumType, AbstractStateMachine<EnumType, AbstractStatePipeline<EnumType>>.State> GetStates(Dictionary<EnumType, PipelineState> elements)
    {
        Dictionary<EnumType, AbstractStateMachine<EnumType, AbstractStatePipeline<EnumType>>.State> stateList = new Dictionary<EnumType, AbstractStateMachine<EnumType, AbstractStatePipeline<EnumType>>.State>();
        foreach (KeyValuePair<EnumType, PipelineState> elementEntry in elements)
        {
            var identifier = elementEntry.Key;
            var element = elementEntry.Value;
            stateList.Add(identifier, new AbstractStateMachine<EnumType, AbstractStatePipeline<EnumType>>.State(element));
        }

        return stateList;
    }

    // Constructor accepts pipeline elements, an initial element, and an Action
    // called when the pipeline advances to the next state.
    //================================================================================
    public AbstractStatePipeline(Dictionary<EnumType, PipelineState> states, EnumType initialState)
    {
        m_states = states;
        m_pipeline = new AbstractStateMachine<EnumType, AbstractStatePipeline<EnumType>>(allStates: GetStates(states), initialState: initialState, stateChangeCallback: OnStateChange);
    }

    public EnumType UpdatePipeline()
    {
        return m_pipeline.UpdateWithValue(this);
    }

    public EnumType CurrentState
    {
        get { return m_pipeline.GetCurrentState(); }
        set { m_pipeline.GoToState(value); }
    }

    public PipelineState GetPipelineElement(EnumType state)
    {
        return m_states[state];
    }

    public PipelineState CurrentPipelineElement
    {
        get { return GetPipelineElement(CurrentState); }
    }

    protected Action<EnumType> StateChangeCallback
    {
        set
        {
            Action<EnumType> callback = OnStateChange;
            callback += value;
            m_pipeline.StateChangeCallback = callback;
        }
    }

    private void OnStateChange(EnumType state)
    {
        GetPipelineElement(state).Reset(this);
    }

    private readonly Dictionary<EnumType, PipelineState> m_states;
    private readonly AbstractStateMachine<EnumType, AbstractStatePipeline<EnumType>> m_pipeline;
}
