using UnityEngine;
using NUnit.Framework;
using Arbiters.Core;

namespace Tests
{
    public class DirectionTests
    {
        [Test]
        public void North()
        {
            Assert.AreEqual(new Vector2(0, 1), Direction.North);
        }

        [Test]
        public void South()
        {
            Assert.AreEqual(new Vector2(0, -1), Direction.South);
        }

        [Test]
        public void East()
        {
            Assert.AreEqual(new Vector2(1, 0), Direction.East);
        }

        [Test]
        public void West()
        {
            Assert.AreEqual(new Vector2(-1, 0), Direction.West);
        }
    }
}