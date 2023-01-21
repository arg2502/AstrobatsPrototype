using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : TacticsMove
{
    GameObject target;

    private void Start() 
    {
        Init();
    }

    private void Update() 
    {
        Debug.DrawRay(transform.position, transform.forward);
        
        if (!turn)
        {
            return;
        }

        if (!moving)
        {
            FindNearestTarget();
            CalculatePath();
            FindSelectableTiles();
            actualTargetTile.target = true;
        }
        else
        {
            Move();
        }
    }

    private void CalculatePath()
    {
        var targetTile = GetTargetTile(target);
        FindPath(targetTile);
    }

    private void FindNearestTarget()
    {
        var targets = GameObject.FindGameObjectsWithTag("Character");

        GameObject nearest = null;
        float distance = Mathf.Infinity;

        foreach(var obj in targets)
        {
            var dist = Vector3.Distance(transform.position, obj.transform.position); // TODO: could replace with Square root magnitude??
            
            if (dist < distance)
            {
                distance = dist;
                nearest = obj;
            }
        }
        Debug.Log($"Setting target to: {nearest}");
        target = nearest;
    }
}
