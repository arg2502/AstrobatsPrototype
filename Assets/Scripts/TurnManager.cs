using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private static Dictionary<string, List<TacticsMove>> units = new Dictionary<string, List<TacticsMove>>();
    private static Queue<string> turnKey = new Queue<string>();
    private static Queue<TacticsMove> turnTeam = new Queue<TacticsMove>();

    private void Update() 
    {
        if (turnTeam.Count == 0)
        {
            InitTeamTurnQueue();
        }
    }

    private static void InitTeamTurnQueue()
    {
        List<TacticsMove> teamList = units[turnKey.Peek()];

        foreach(var unit in teamList)
        {
            turnTeam.Enqueue(unit);
        }

        StartTurn();
    }

    private static void StartTurn()
    {
        Debug.Log($"START TURN: turn team count: {turnTeam.Count}");
        if (turnTeam.Count > 0)
        {
            turnTeam.Peek().BeginTurn();
        }
    }

    public static void EndTurn()
    {
        var unit = turnTeam.Dequeue();
        unit.EndTurn();

        if (turnTeam.Count > 0)
        {
            StartTurn();
        }
        else
        {
            string team = turnKey.Dequeue();
            turnKey.Enqueue(team);
            InitTeamTurnQueue();
        }
    }

    public static void AddUnit(TacticsMove unit)
    {
        List<TacticsMove> list;

        if (!units.ContainsKey(unit.tag))
        {
            list = new List<TacticsMove>();
            units.Add(unit.tag, list);
            // units[unit.tag] = list;

            if (!turnKey.Contains(unit.tag))
            {
                turnKey.Enqueue(unit.tag);
            }
        }
        else
        {
            list = units[unit.tag];
        }

        list.Add(unit);
    }
}
