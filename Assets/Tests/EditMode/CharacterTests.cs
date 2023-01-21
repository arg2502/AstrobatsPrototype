using NUnit.Framework;
using Arbiters.Core;
using UnityEngine;
using UnityEditor;

namespace Tests
{
    public class CharacterTests
    {
        private string ZeebeeName => "Zeebee";
        private string TwiggsName => "Twiggs";
        private string WhiteChickaName => "WhiteChicka";
        private string CrabgrassName => "Crabgrass";
        private int OneStarTotal => 5;
        private int TwoStarTotal => 6;
        private int ThreeStarTotal => 7;

        private CharacterData LoadCharacter(string characterName)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(CharacterData)} {characterName}");

            if (guids.Length == 0)
            {
                Assert.Fail($"No object found of name: {characterName}");
            }
            if (guids.Length > 1)
            {
                Debug.LogWarning($"More than one object of name ({characterName}) was found, taking the first one");
            }

            return (CharacterData)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(CharacterData));
        }

        private void StatsCheck(string characterName)
        {
            var denigen = LoadCharacter(characterName);

            int expectedSum = 0;
            switch (denigen.star)
            {
                case StarValue.OneStar: expectedSum = OneStarTotal; break;
                case StarValue.TwoStar: expectedSum = TwoStarTotal; break;
                case StarValue.ThreeStar: expectedSum = ThreeStarTotal; break;
                default: break;
            }

            if (expectedSum <= 0)
            {
                Assert.Fail($"Could not find the expected total for star value: {denigen.star}");
            }

            int denigenTotalStats = denigen.speedStat + denigen.powerStat;
            Assert.AreEqual(expectedSum, denigenTotalStats);
        }

        [Test]
        public void ZeebeeStarCheck()
        {
            var denigen = LoadCharacter(ZeebeeName);
            Assert.AreEqual(StarValue.TwoStar, denigen.star);
        }

        [Test]
        public void TwiggsStarCheck()
        {
            var denigen = LoadCharacter(TwiggsName);
            Assert.AreEqual(StarValue.TwoStar, denigen.star);
        }

        [Test]
        public void WhiteChickaStarCheck()
        {
            var denigen = LoadCharacter(WhiteChickaName);
            Assert.AreEqual(StarValue.ThreeStar, denigen.star);
        }
        
        [Test]
        public void CrabgrassStarCheck()
        {
            var denigen = LoadCharacter(CrabgrassName);
            Assert.AreEqual(StarValue.TwoStar, denigen.star);
        }

        [Test]
        public void ZeebeeStatsCheck()
        {
            StatsCheck(ZeebeeName);
        }

        [Test]
        public void TwiggsStatsCheck()
        {
            StatsCheck(TwiggsName);
        }

        [Test]
        public void WhiteChickaStatsCheck()
        {
            StatsCheck(WhiteChickaName);
        }

        [Test]
        public void CrabgrassStatsCheck()
        {
            StatsCheck(CrabgrassName);
        }

    }
}