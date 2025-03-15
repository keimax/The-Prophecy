using UnityEngine;
using TheProphecy.Interfaces;
using UnityEngine.UI;
using TheProphecy.LevelRun;
using Random = UnityEngine.Random;

namespace TheProphecy.Enemy
{
    public class BaseEnemy : BaseUnit, IDamageable
    {
        [Header("Drop Variables")]
        [SerializeField] private int _minCoinDropRate = 0;
        [SerializeField] private int _maxCoinDropRate = 5;

        public override void Start()
        {
            base.Start(); // Ensure health and shield are initialized from BaseUnit
            // Initialize UI elements for health bar if needed
        }

        public  void OnTakeDamage(int damage) // Use override here
        {
            // Handle damage logic
            health -= damage; // Reduce health by damage amount
            UpdateHealthBar(); // Update health bar UI

            if (health <= 0)
            {
                Die(); // Call die method if health is zero
            }
        }

        protected override void Die()
        {
            DropLoot();
            base.Die(); // Call the base die method
            gameObject.SetActive(false); // Deactivate the enemy
            UpdateLevelStats(); // Update level stats upon death
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