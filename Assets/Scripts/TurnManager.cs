using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private static List<Player> players = new List<Player>();
    private static Queue<Player> turnOrder = new Queue<Player>();

    private void Update()
    {
        if (turnOrder.Count == 0)
        {
            Init();
        }
    }

    public static void Init()
    {
        foreach(var player in players)
        {
            turnOrder.Enqueue(player);
        }

        StartTurn();
    }

    public static void AddUnit(Player player, bool first) // TODO: make first something more robust in terms of priority
    {
        if (first)
        {
            players.Insert(0, player);
        }
        else
        { 
            players.Add(player);
        }
        
    }

    private static void StartTurn()
    {
        Debug.Log($"START TURN: turn team count: {turnOrder.Count}");
        if (turnOrder.Count > 0)
        {
            turnOrder.Peek().BeginTurn();
        }
    }

    public static void EndTurn()
    {
        Debug.Log($"Turn Manager END TURN: turn team count: {turnOrder.Count}");
        var unit = turnOrder.Dequeue();
        //unit.EndTurn();
        turnOrder.Enqueue(unit);
        StartTurn();
    }

}
