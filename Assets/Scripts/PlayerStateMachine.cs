using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Begin,
    SelectMove,
    Moving,
    SelectAttack,
    Attacking,
    End
}

public class PlayerStateMachine : AbstractStatePipeline<PlayerState>
{
    public Player player;

    private static PipelineElement CreatePipelineElement(PlayerState state)
    {
        switch(state)
        {
            case PlayerState.Idle: return new IdleState();
            case PlayerState.Begin: return new BeginState();
            case PlayerState.SelectMove: return new SelectMoveState();
            case PlayerState.Moving: return new MovingState();
            //case PlayerState.SelectAttack: return new SelectAttackState();
            //case PlayerState.Attacking: return new AttackingState();
            case PlayerState.End: return new EndState();
            default: return null;
        }
    }
    private static Dictionary<PlayerState, PipelineState> CreatePipeline()
    {
        var pipeline = new Dictionary<PlayerState, PipelineState>();
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            pipeline.Add(state, CreatePipelineElement(state));
        }
        return pipeline;
    }

    public PlayerStateMachine(Player _player) : base(states: CreatePipeline(), initialState: PlayerState.Idle)
    {
        player = _player;
        StateChangeCallback = (state) =>
        {
            player.CurrentPlayerState = state;
        };
    }

    private abstract class PipelineElement : PipelineState
    {
        protected Player player { get; private set; }

        public override void Reset(AbstractStatePipeline<PlayerState> pipeline)
        {
            player = ((PlayerStateMachine)pipeline).player;
        }

        public override bool ShouldTransition(AbstractStatePipeline<PlayerState> pipeline)
        {
            return false;
        }

        public override void HandleTransition(StateTransitionPhase transitionPhase)
        {
            if (transitionPhase == StateTransitionPhase.Entered)
            {
                OnEnter();
            }
            else if (transitionPhase == StateTransitionPhase.Exited)
            {
                OnExit();
            }
        }

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }

    }

    private class IdleState : PipelineElement
    {
        private bool didBeginTurn = false;

        public override PlayerState NextState(AbstractStatePipeline<PlayerState> pipeline)
        {
            return PlayerState.Begin;
        }

        public override bool ShouldTransition(AbstractStatePipeline<PlayerState> pipeline)
        {
            return didBeginTurn;
        }

        protected override void OnEnter() 
        {
            Debug.Log($"Idle State: {player}");
            player.OnBeganTurn?.AddListener(OnBeganTurn);
        }

        protected override void OnExit()
        {
            player.OnBeganTurn?.RemoveListener(OnBeganTurn);
            didBeginTurn = false;
        }

        private void OnBeganTurn()
        {
            didBeginTurn = true;
        }
    }

    private class BeginState : PipelineElement 
    {
        public override PlayerState NextState(AbstractStatePipeline<PlayerState> pipeline)
        {
            return PlayerState.SelectMove;
        }

        public override bool ShouldTransition(AbstractStatePipeline<PlayerState> pipeline)
        {
            return true;
        }

        protected override void OnEnter()
        {
            Debug.Log($"Begin Turn for Player: {player}");
        }
    }

    private class SelectMoveState : PipelineElement
    {
        private bool beganMoving = false;
        public override PlayerState NextState(AbstractStatePipeline<PlayerState> pipeline)
        {
            return PlayerState.Moving;
        }

        public override bool ShouldTransition(AbstractStatePipeline<PlayerState> pipeline)
        {
            return beganMoving;
        }

        protected override void OnEnter()
        {
            Debug.Log($"Select Move State: {player}");
            player.OnBeganMove?.AddListener(OnBeganMove);
        }

        protected override void OnExit()
        {
            player.OnBeganMove?.RemoveListener(OnBeganMove);

            beganMoving = false;
        }

        private void OnBeganMove(TacticsMove tacticsMove)
        {
            beganMoving = true;
        }
    }

    private class MovingState : PipelineElement
    {
        private bool endedMoving = false;

        public override PlayerState NextState(AbstractStatePipeline<PlayerState> pipeline)
        {
            return PlayerState.End; // TODO: return PlayerState.SelectAttack;
        }

        public override bool ShouldTransition(AbstractStatePipeline<PlayerState> pipeline)
        {
            return endedMoving;
        }

        protected override void OnEnter()
        {
            Debug.Log($"Moving State: {player}");
            player.OnEndMove?.AddListener(OnEndedMove);
        }

        protected override void OnExit()
        {
            player.OnEndMove?.RemoveListener(OnEndedMove);

            endedMoving = false;
        }

        private void OnEndedMove(TacticsMove tacticsMove)
        {
            endedMoving = true;
        }
    }

    private class EndState : PipelineElement
    {
        public override PlayerState NextState(AbstractStatePipeline<PlayerState> pipeline)
        {
            return PlayerState.Idle;
        }

        public override bool ShouldTransition(AbstractStatePipeline<PlayerState> pipeline)
        {
            return true;
        }

        protected override void OnEnter()
        {
            Debug.Log($"End State: {player}");

            player.EndTurn();
        }

        protected override void OnExit()
        {
            Debug.Log($"Leaving End State -- {player}");

        }
    }
}
