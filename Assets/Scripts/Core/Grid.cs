using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Arbiters.Core
{
    public static class Grid
    {
        public static int GridVerticalSize => 12;
        public static int GridHorizontalSize => 12;

        public static Vector2 MinGridPosition => Vector2.zero;
        public static Vector2 MaxGridPosition => new Vector2(GridHorizontalSize - 1, GridVerticalSize - 1);

        public static List<Vector2> OccupiedPositions { get; private set; } = new List<Vector2>();

        public static UnityEvent<Vector2> OnPositionOccupied = new UnityEvent<Vector2>();
        public static void PositionOccupied(Vector2 occupiedPosition)
        {
            if (OccupiedPositions.Contains(occupiedPosition))
            {
                // TODO: determine how to handle collisions
                return;
            }

            OccupiedPositions.Add(occupiedPosition);
            OnPositionOccupied?.Invoke(occupiedPosition);
        }

        public static Vector2 GetRandomPosition()
        {
            return new Vector2((int)Random.Range(MinGridPosition.x, MaxGridPosition.x), (int)Random.Range(MinGridPosition.y, MaxGridPosition.y));
        }

        public static List<Direction> GetPossibleDirections(Vector2 currentPosition, List<Direction> movesSoFar)
        {
            var possibleMoves = new List<Direction>();

            if (IsSpaceAvailable(currentPosition, Direction.North, movesSoFar))
            {
                possibleMoves.Add(Direction.North);
            }

            if (IsSpaceAvailable(currentPosition, Direction.South, movesSoFar))
            {
                possibleMoves.Add(Direction.South);
            }

            if (IsSpaceAvailable(currentPosition, Direction.East, movesSoFar))
            {
                possibleMoves.Add(Direction.East);
            }

            if (IsSpaceAvailable(currentPosition, Direction.West, movesSoFar))
            {
                possibleMoves.Add(Direction.West);
            }

            return possibleMoves;
        }

        private static bool IsSpaceAvailable(Vector2 currentPosition, Direction direction, List<Direction> movesSoFar)
        {
            Vector2 desiredPosition = currentPosition + direction.Vector;
            bool onBoard = (desiredPosition.y <= MaxGridPosition.y && desiredPosition.y >= MinGridPosition.y
                            && desiredPosition.x <= MaxGridPosition.x && desiredPosition.x >= MinGridPosition.x);
            bool desiredSpaceIsNotOccupied = (!OccupiedPositions.Contains(desiredPosition)); // TODO: allow it if the occupant is another character
            bool lastMoveNotOpposite = (movesSoFar.LastOrDefault() != Direction.Opposite(direction));
            Debug.Log($"For direction: ({direction}), desiredPosition: {desiredPosition}, onBoard: {onBoard}, desiredSpaceIsNotOccupied: {desiredSpaceIsNotOccupied}, lastMoveNotOpposite: {lastMoveNotOpposite}");
            return (onBoard && desiredSpaceIsNotOccupied && lastMoveNotOpposite);
        }
    }
}