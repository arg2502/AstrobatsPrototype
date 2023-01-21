using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arbiters.Core
{
    public enum StarValue
    {
        Unknown,
        OneStar,
        TwoStar,
        ThreeStar
    }

    [CreateAssetMenu(fileName = "CharacterData", menuName = "AstrobatsPrototype/Character", order = 0)]
    public class CharacterData : ScriptableObject 
    {
        public StarValue star;
        [Range(1, 6)] public int powerStat;
        [Range(1, 6)] public int speedStat;
    }
}
