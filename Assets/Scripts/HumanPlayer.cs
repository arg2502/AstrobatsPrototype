using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    protected override void UpdateSelectMove()
    {
        CheckMouse();
    }

    private void CheckMouse()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Character" || hit.collider.tag == "NPC") // temp
                {
                    Debug.Log($"Clicked a character");
                    TacticsMove selected = hit.collider.GetComponent<TacticsMove>();
                    if (teamMembers.Contains(selected))
                    {
                        SelectPiece(selected);
                    }
                }
                else if (hit.collider.tag == "Tile")
                {
                    Debug.Log($"Clicked a tile");
                    if (currentlySelected == null)
                    {
                        Debug.Log($"no character selected");
                    }
                    else
                    {
                        Tile t = hit.collider.GetComponent<Tile>();

                        if (t.selectable)
                        {
                            currentlySelected?.MoveToTile(t);
                            OnBeganMove?.Invoke(currentlySelected);
                        }
                    }
                }
            }
        }
    }
}
