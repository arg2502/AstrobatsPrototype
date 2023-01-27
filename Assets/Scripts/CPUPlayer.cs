using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CPUPlayer : Player
{
    protected override void UpdateSelectMove()
    {
        SelectPiece(GetPiece());
    }

    private TacticsMove GetPiece()
    {
        // TODO: create cool AI for determining which character to move
        return teamMembers[Random.Range(0, teamMembers.Count)];

    }
}
