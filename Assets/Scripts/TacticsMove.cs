using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    public bool selected = false;
    public bool moving = false;
    public int move = 5; // TODO: replace with speed stat
    public float jumpHeight = 2f; // probably not needed, but we'll see
    public float moveSpeed = 2f;
    public float jumpVelocity = 4.5f;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();

    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] tiles;
    Stack<Tile> path = new Stack<Tile>();
    Tile currentTile;
    float halfHeight = 0f;

    private bool fallingDown = false;
    private bool jumpingUp = false;
    private bool movingEdge = false;

    Vector3 jumpTarget;

    protected Tile actualTargetTile;

    [SerializeField] protected Player playerParent;

    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        halfHeight = GetComponent<Collider>().bounds.extents.y;
    }

    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>();
        }

        return tile;
    }

    public void ComputeAdjacencyLists(float jumpHt, Tile target)
    {
        foreach(var tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(jumpHt, target);
        }
    }

    public void FindSelectableTiles()
    {
        ComputeAdjacencyLists(jumpHeight, null);
        GetCurrentTile();

        // BFS
        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue(currentTile);
        currentTile.visited = true;
        currentTile.parent = null;

        while (process.Count > 0)
        {
            Tile tile = process.Dequeue();

            selectableTiles.Add(tile);
            tile.selectable = true;

            if (tile.distance < move)
            {
                foreach(var adjTile in tile.adjacencyList)
                {
                    if (!adjTile.visited)
                    {
                        adjTile.parent = tile;
                        adjTile.visited = true;
                        adjTile.distance = 1 + tile.distance;
                        process.Enqueue(adjTile);
                    }
                }
            }
        }
    }

    public void MoveToTile(Tile tile)
    {
        path.Clear();
        tile.target = true;
        moving = true;

        Tile next = tile;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    public void Move()
    {
        if (path.Count > 0)
        {
            var tile = path.Peek();
            var target = tile.transform.position;

            // Calculate the unit's position on top of the target tile
            target.y += halfHeight + tile.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    Jump(target);
                }
                else
                {
                    CalculateHeading(target);
                    SetHorizontalVelocity();
                }

                // Locomotion -- act of moving
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                // Tile center reached
                transform.position = target;
                path.Pop();
            }
        }
        else
        {
            RemoveSelectableTiles();
            playerParent.OnEndMove?.Invoke(this);
            moving = false;
            //TurnManager.EndTurn();
        }
    }

    protected void RemoveSelectableTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach(var tile in selectableTiles)
        {
            tile.Reset();
        }

        selectableTiles.Clear();
    }

    private void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    private void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    private void Jump(Vector3 target)
    {
        // simple state machine
        if (fallingDown)
        {  
            FallDownward(target);
        }
        else if (jumpingUp)
        {
            JumpUpward(target);
        }
        else if (movingEdge)
        {
            MoveToEdge();
        }
        else
        {
            PrepareJump(target);
        }
    }

    private void PrepareJump(Vector3 target)
    {
        float targetY = target.y;

        target.y = transform.position.y;

        CalculateHeading(target);

        if (transform.position.y > targetY)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = true;

            jumpTarget = transform.position + (target - transform.position) / 2f;
        }
        else
        {
            fallingDown = false;
            jumpingUp = true;
            movingEdge = false;

            velocity = heading * moveSpeed / 3f;

            float difference = targetY - transform.position.y;

            velocity.y = jumpVelocity * (0.5f + difference / 2f);
        }
    }

    private void FallDownward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y <= target.y)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = false;

            var pos = transform.position;
            pos.y = target.y;
            transform.position = pos;

            velocity = Vector3.zero;
        }
    }

    private void JumpUpward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y > target.y)
        {
            jumpingUp = false;
            fallingDown = true;
        }
    }

    private void MoveToEdge()
    {
        if (Vector3.Distance(transform.position, jumpTarget) >= 0.05f)
        {
            SetHorizontalVelocity();
        }
        else
        {
            movingEdge = false;
            fallingDown = true;

            velocity /= 6f;
            velocity.y = 1.5f; // little hop
        }
    }

    //public void BeginTurn()
    //{
    //    selected = true;
    //}

    //public void EndTurn()
    //{
    //    selected = false;
    //}

    protected Tile FindLowestF(List<Tile> list)
    {
        var lowest = list[0];

        foreach (var t in list)
        {
            if (t.f < lowest.f)
            {
                lowest = t;
            }
        }

        list.Remove(lowest);

        return lowest;
    }

    protected Tile FindEndTile(Tile t)
    {
        Stack<Tile> tempPath = new Stack<Tile>();

        Tile next = t.parent;
        while (next != null)
        {
            tempPath.Push(next);
            next = next.parent;
        }

        // if it's within our move range, just move to the parent
        if (tempPath.Count <= move)
        {
            return t.parent;
        }

        Tile endTile = null;
        for (int i = 0; i <= move; i++)
        {
            endTile = tempPath.Pop();
        }

        return endTile;
    }

    protected void FindPath(Tile target)
    {
        ComputeAdjacencyLists(jumpHeight, target);
        GetCurrentTile();

        var openList = new List<Tile>();
        var closedList = new List<Tile>();

        openList.Add(currentTile);
        currentTile.h = Vector3.Distance(currentTile.transform.position, target.transform.position);
        currentTile.f = currentTile.h;

        while (openList.Count > 0)
        {
            var t = FindLowestF(openList);
            closedList.Add(t);

            if (t == target)
            {
                // We've reached our goal
                // But we want to stand NEXT TO the target
                actualTargetTile = FindEndTile(t);
                MoveToTile(actualTargetTile);
                playerParent.OnBeganMove?.Invoke(this);
                return;
            }

            foreach(var tile in t.adjacencyList)
            {
                if (closedList.Contains(tile))
                {
                    // Do nothing, already processed this tile and found the fastest way
                }
                else if (openList.Contains(tile))
                {
                    // We've seen this tile before
                    // Check and see if we've found a faster way to the tile

                    float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    if (tempG < tile.g)
                    {
                        tile.parent = t;

                        tile.g = tempG;
                        tile.f = tile.g + tile.h;
                    }
                }
                else
                {
                    // This is the first time we're visiting this tile

                    tile.parent = t;
                    tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.h = Vector3.Distance(tile.transform.position, target.transform.position);
                    tile.f = tile.g + tile.h;

                    openList.Add(tile);
                }
            }
        }

        // TODO: what do you do if there is no path to the target tile?
        Debug.Log("Path not found");
    }

    protected void Attack(Tile opponentTile)
    {

    }
}
