using UnityEngine;
using TheProphecy.Interfaces;
using UnityEngine.UI;
using TheProphecy.LevelRun;
using Random = UnityEngine.Random;

namespace TheProphecy.Enemy
{
    public class BaseEnemy : BaseUnit, IDamageable
    {
        [Header(" Variables")]
        [SerializeField] private int _minCoinDropRate = 0;
        [SerializeField] private int _maxCoinDropRate = 5;
        [SerializeField] private int _minXPDrop = 1;
        [SerializeField] private int _maxXPDrop = 100;

        [SerializeField] private GameObject _hitPrefab;
        [SerializeField] private GameObject _explodePrefab;
        [SerializeField] private GameObject _smokePrefab;

        private bool _xpDropped = false; // Flag to track if XP has been dropped
        private bool _isDead = false; // Flag to track if the enemy has died

        public override void Start()
        {
            base.Start(); // Ensure health and shield are initialized from BaseUnit
                          // Initialize UI elements for health bar if needed
        }

        public void OnTakeDamage(int damage) // Use override here
        {
            // Handle damage logic
            health -= damage; // Reduce health by damage amount
            if (_hitPrefab != null)
                GameObject.Destroy(GameObject.Instantiate(_hitPrefab, transform.position, Quaternion.identity), 5f);
            UpdateHealthBar(); // Update health bar UI

            if (_smokePrefab)
            {
                if (health <= MaxHealth / 2)
                {
                    _smokePrefab.SetActive(true);
                }
            }

            if (health <= 0)
            {
             
                Die(); // Call die method if health is zero
            }
        }

        private void DropXP()
        {
            if (_xpDropped) return; // Ensure XP is only dropped once

            PlayerXP playerXP = (PlayerXP)FindAnyObjectByType(typeof(PlayerXP));
            if (playerXP != null)
            {
                int xpToDrop = Random.Range(_minXPDrop, _maxXPDrop + 1); // Generate random XP within the range
                Debug.Log($"Dropping {xpToDrop} XP");
                playerXP.EarnXP(xpToDrop);
            }

            _xpDropped = true; // Set the flag to true after dropping XP
        }

        protected override void Die()
        {
            if (_isDead) return; // Ensure Die is only called once

            if (_explodePrefab != null)
                GameObject.Destroy(GameObject.Instantiate(_explodePrefab, transform.position, Quaternion.identity), 1f);
            DropLoot();
            DropXP();
            base.Die(); // Call the base die method
            gameObject.SetActive(false); // Deactivate the enemy
            UpdateLevelStats(); // Update level stats upon death

            _isDead = true; // Set the flag to true after dying
        }

        private void DropLoot()
        {
            Vector2 enemyDirection = (Vector2)transform.up; // Use the enemy's up direction
            var lootDropManager = GameObject.FindAnyObjectByType<LootDropManager>();
            if (lootDropManager != null)
            {
                lootDropManager.DropLoot(transform, enemyDirection);
            }
            else
            {
                Debug.LogWarning("LootDropManager not found in the scene.");
            }
        }

        private void UpdateLevelStats()
        {
            LevelRunStats levelStats = LevelManager.instance.levelRunStats;
            levelStats.AddKill();
            levelStats.AddCoins(Random.Range(_minCoinDropRate, _maxCoinDropRate));
        }

        protected override void UpdateHealthBar()
        {
            base.UpdateHealthBar(); // Call the base method to update health bar UI
                                    // Additional logic for enemy health bar if needed
        }

        // Implement the IDamageable interface method if needed
        public void ReceiveDamage(int damage)
        {
            OnTakeDamage(damage); // Call the OnTakeDamage method
        }

        // Add any other necessary methods for enemy behavior
    }
}