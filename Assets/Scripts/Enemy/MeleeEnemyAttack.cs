using System.Collections;
using UnityEngine;
using TheProphecy.Player; // Include the BasePlayer namespace

namespace TheProphecy.Enemy
{
    public class MeleeEnemyAttack : MonoBehaviour
    {
        [SerializeField] private int _damage = 1;
        [SerializeField][Range(0f, 1f)] private float _damageDelay = 0.5f;
        private float _damageDelayCounter = 0f;

        private void Update()
        {
            // Increment the damage delay counter
            _damageDelayCounter += Time.deltaTime;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            // Check if the collided object has the "Player" tag
            if (collision.CompareTag("Player"))
            {
                var player = collision.GetComponent<BasePlayer>();

                if (player != null)
                {
                    // Check if enough time has passed to apply damage
                    if (_damageDelayCounter >= _damageDelay)
                    {
                        Debug.Log("Attacking player!");
                        player.TakeDamage(_damage); // Apply damage using TakeDamage method
                        _damageDelayCounter = 0; // Reset the counter after attacking
                    }
                }
                else
                {
                    Debug.Log("Player component not found.");
                }
            }
        }
    }
}