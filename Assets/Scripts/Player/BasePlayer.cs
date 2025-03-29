using System;
using UnityEngine;

namespace TheProphecy.Player
{
    public class BasePlayer : BaseUnit
    {
        public event Action OnHealthChanged; // Event to notify health changes
        public event Action OnShieldChanged; // Event to notify shield changes



        [SerializeField] private GameObject _hitPrefab;
        [SerializeField] private GameObject _explodePrefab;
        [SerializeField] private GameObject _smokePrefab;


        public override void Start()
        {
            base.Start(); // Calls the BaseUnit Start method
            health = MaxHealth; // Set starting health to max
            shield = MaxShield; // Set starting shield to max
            Debug.Log($"BasePlayer initialized with Health: {health}, Shield: {shield}");
        }

        public void Resurrect()
        {
            health = MaxHealth; // Reset health to max on resurrection
            shield = MaxShield; // Reset shield to max on resurrection
            OnHealthChanged?.Invoke(); // Notify health change
            OnShieldChanged?.Invoke(); // Notify shield change

         
                _smokePrefab.SetActive(false);

        }

        public void GiveHealth(int amount)
        {
            health = Mathf.Clamp(health + amount, 0, MaxHealth);
            OnHealthChanged?.Invoke(); // Notify health change
            Debug.Log($"Health updated: {health}/{MaxHealth}");


            if (health > MaxHealth / 3)
            {
                _smokePrefab.SetActive(false);
            }

        }

        public void GiveShield(int amount)
        {
            shield = Mathf.Clamp(shield + amount, 0, MaxShield);
            OnShieldChanged?.Invoke(); // Notify shield change
            Debug.Log($"Shield updated: {shield}/{MaxShield}");
        }

        public void TakeDamage(int damage)
        {
            GameObject.Destroy(GameObject.Instantiate(_hitPrefab, transform.position, Quaternion.identity), 1f);

            if (shield > 0)
            {
                int shieldDamage = Mathf.Min(damage, shield);
                shield -= shieldDamage;
                damage -= shieldDamage;
                OnShieldChanged?.Invoke(); // Notify shield change
            }

            if (damage > 0)
            {
                health = Mathf.Max(health - damage, 0); // Ensure health does not go below 0
                OnHealthChanged?.Invoke(); // Notify health change
            }

            Debug.Log($"Damage taken: {damage}, Remaining Health: {health}, Remaining Shield: {shield}");

            if(health <= MaxHealth/3)
            {
               _smokePrefab.SetActive(true);
            }


            // Check if the player has died
            if (health <= 0)
            {
                Die(); // Call the Die method
            }
        }

        // Override the Die method if necessary
        protected override void Die()
        {
            GameObject.Destroy(GameObject.Instantiate(_explodePrefab, transform.position, Quaternion.identity), 2f);
            base.Die(); // Call the base die logic
            Debug.Log($"{gameObject.name} has died."); // Additional logging

            // Find the UIController and toggle the death screen
            UIController uiController = FindObjectOfType<UIController>();
            if (uiController != null)
            {
                uiController.ToggleDeathScreen(true); // Show the death screen
            }
            else
            {
                Debug.LogWarning("UIController not found!");
            }
        }
    }
}