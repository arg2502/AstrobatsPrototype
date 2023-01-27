using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public PlayerState CurrentPlayerState { get; set; }
    public UnityEvent OnBeganTurn { get; set; } = new UnityEvent();
    public UnityEvent<TacticsMove> OnBeganMove { get; set; } = new UnityEvent<TacticsMove>();
    public UnityEvent<TacticsMove> OnEndMove { get; set; } = new UnityEvent<TacticsMove>();
    //public UnityEvent OnEndedTurn { get; set; } = new UnityEvent();

    protected TacticsMove currentlySelected = null;
    private PlayerStateMachine stateMachine;
    [SerializeField] private bool first; // temp
    [SerializeField] protected List<TacticsMove> teamMembers;

    private void Awake()
    {
        stateMachine = new PlayerStateMachine(this);
    }

    private void Start()
    {
        TurnManager.AddUnit(this, first);
    }

    private void Update()
    {
        stateMachine.UpdatePipeline();

        switch (CurrentPlayerState)
        {
            case (PlayerState.Idle): UpdateIdle(); break;
            case (PlayerState.Begin): UpdateBegin(); break;
            case (PlayerState.SelectMove): UpdateSelectMove(); break;
            case (PlayerState.Moving): UpdateMoving(); break;
            case (PlayerState.SelectAttack): UpdateSelectAttack(); break;
            case (PlayerState.Attacking): UpdateAttacking(); break;
            case (PlayerState.End): UpdateEnd(); break;
            default: break;
        }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateBegin() { }
    protected virtual void UpdateSelectMove() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateSelectAttack() { }
    protected virtual void UpdateAttacking() { }
    protected virtual void UpdateEnd() { }

    public void BeginTurn()
    {
        Debug.LogError($"{gameObject} BEGIN TURN");
        OnBeganTurn?.Invoke();
    }

    public void EndTurn()
    {
        Debug.Log("Player end turn");
        DeselectPiece();
        TurnManager.EndTurn();
    }

    protected void SelectPiece(TacticsMove piece)
    {
        DeselectPiece();
        if (piece == null)
        {
            Debug.LogError("Tried to select a null piece");
            return;
        }
        currentlySelected = piece;
        currentlySelected.selected = true;
    }

    protected void DeselectPiece()
    {
        if (currentlySelected != null) 
        { 
            currentlySelected.selected = false;
            currentlySelected = null;
        }
    }

    

}
