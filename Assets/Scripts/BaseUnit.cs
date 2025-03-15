using System.Collections;
using UnityEngine;

public abstract class BaseUnit : MonoBehaviour
{
    [Header("Health Variables")]
    [SerializeField] protected int MAX_HEALTH = 100; // Maximum health value
    [SerializeField] private int _health; // Backing field for health
    protected bool isAlive = true;

    [Header("Shield Variables")]
    [SerializeField] protected int MAX_SHIELD = 50; // Maximum shield value
    [SerializeField] private int _shield; // Backing field for shield

    // Make these properties public and serialized to show in Inspector
    public int health
    {
        get => _health;
        protected set // Protected setter
        {
            _health = Mathf.Clamp(value, 0, MAX_HEALTH);
            UpdateHealthBar(); // Update health bar when health changes
            Debug.Log($"Health set to: {_health}"); // Added logging
        }
    }

    public int shield
    {
        get => _shield;
        protected set // Protected setter
        {
            _shield = Mathf.Clamp(value, 0, MAX_SHIELD);
            UpdateShieldBar(); // Update shield bar when shield changes
            Debug.Log($"Shield set to: {_shield}"); // Added logging
        }
    }

    public int MaxHealth => MAX_HEALTH; // Public getter for MAX_HEALTH
    public int MaxShield => MAX_SHIELD; // Public getter for MAX_SHIELD

    public virtual void Start()
    {
        health = MAX_HEALTH; // Initialize health
        shield = MAX_SHIELD; // Initialize shield
        Debug.Log($"BaseUnit initialized with Health: {health}, Shield: {shield}");
    }

    protected virtual void UpdateHealthBar() { /* To be overridden in derived classes */ }
    protected virtual void UpdateShieldBar() { /* To be implemented if needed */ }

    protected virtual void Die()
    {
        isAlive = false; // Set alive status to false
        gameObject.SetActive(false); // Deactivate the object
        Debug.Log($"{gameObject.name} has died.");
    }
}