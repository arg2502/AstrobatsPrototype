using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool walkable = true;
    public bool current = false;
    public bool target = false;
    public bool selectable = false;

    private Material material;

    public List<Tile> adjacencyList = new List<Tile>();

    // needed BFS (breadth first search)
    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;

    // A* (path search)
    public float f = 0;
    public float g = 0;
    public float h = 0;

    public void Reset() 
    {
        adjacencyList.Clear();

        current = false;
        target = false;
        selectable = false;

        visited = false;
        parent = null;
        distance = 0;

        f = g = h = 0;
    }

    public void FindNeighbors(float jumpHeight, Tile target)
    {
        Reset();

        CheckTile(Vector3.forward, jumpHeight, target);
        CheckTile(-Vector3.forward, jumpHeight, target);
        CheckTile(Vector3.right, jumpHeight, target);
        CheckTile(-Vector3.right, jumpHeight, target);
    }

    public void CheckTile(Vector3 direction, float jumpHeight, Tile target)
    {
        // jumpHeight, if we want to add a 3rd dimension, some players can "jump" to a tile that's higher
        // might be interesting...

        Vector3 halfExtents = new Vector3(0.25f, (1 + jumpHeight) / 2f, 0.25f); 
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach(var item in colliders)
        {
            var tile = item.GetComponent<Tile>();
            if (tile != null && tile.walkable) // TODO: this might need to be changed if someone's passive is to walk on nonwalkable tiles
            {
                // see if something's on the tile already
                RaycastHit hit;

                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1) || (tile == target))
                {
                    adjacencyList.Add(tile);
                }
            }
        }
    }

    private void Awake() 
    {
        material = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        SetColor();
    }

    private void SetColor()
    {
        if (current)
        {
            material.color = Color.magenta;
        }
        else if (target)
        {
            material.color = Color.green;
        }
        else if (selectable)
        {
            material.color = Color.red;
        }
        else
        {
            material.color = Color.white;
        }
    }
}
