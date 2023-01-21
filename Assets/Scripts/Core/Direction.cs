using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arbiters.Core
{
    public class Direction
    {
        public Vector2 Vector { get; private set;}
        public Direction(Vector2 dir)
        {
            Vector = dir;
        }

        public override string ToString()
        {
            return Vector.ToString();
        }
        public static readonly Direction North = new Direction(Vector2.up);
        public static readonly Direction South = new Direction(Vector2.down);
        public static readonly Direction East = new Direction(Vector2.right);
        public static readonly Direction West = new Direction(Vector2.left);

        public static Direction Opposite(Direction originalDir)
        {
            if(originalDir == North)
            {
                return South;
            }
            else if (originalDir == South)
            {
                return North;
            }
            else if (originalDir == East)
            {
                return West;
            }
            else if (originalDir == West)
            {
                return East;
            }
            else return South;
        }

        
    }
}