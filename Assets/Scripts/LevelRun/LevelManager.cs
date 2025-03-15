using System.Collections;
using System.Collections.Generic;
using TheProphecy.Map.DungeonGeneration;
using TheProphecy.Player; // Ensure this namespace is included
using UnityEngine;

namespace TheProphecy.LevelRun
{
    public class LevelManager : Singleton<LevelManager>
    {
        [SerializeField] private RandomWalkDungeonGenerator _levelGenerator;
        [SerializeField] private BasePlayer _basePlayer;

        public LevelRunStats levelRunStats;

        protected override void Awake()
        {
            base.Awake();
            levelRunStats = new LevelRunStats();
            _levelGenerator.GenerateDungeon(); // Generate the initial dungeon
        }

        public void ResetLevel()
        {
            _basePlayer.gameObject.SetActive(true); // Activate the player
            _basePlayer.Resurrect(); // Reset player stats
            levelRunStats.Reset(); // Reset level run stats
            _levelGenerator.GenerateDungeon(); // Regenerate the dungeon
        }
    }
}