using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arbiters.Core
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private CharacterData data;
        private Vector2 gridPosition;

        private void Start() 
        {
            gridPosition = Grid.GetRandomPosition();

            // testing -- determine possible moves
            Debug.Log($"possible moves starting from position: {gridPosition}");
            List<Direction> movesSoFar = new List<Direction>();

            var possibleMoves = Grid.GetPossibleDirections(gridPosition, movesSoFar);
            possibleMoves.ForEach(move => Debug.Log($"move: {move}"));
        }
    }
}