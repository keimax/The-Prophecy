using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Catapult : MonoBehaviour
{
    [SerializeField] private List<GameObject> CargoLoad = new List<GameObject>();
    [SerializeField] private float launchForce = 500f;
    [SerializeField] private float fireDelay = 2f;

    [Header("Detection Parameters")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool canFire = true;

    private void FixedUpdate()
    {
        // Only check for player if we can still fire
        if (canFire)
        {
            // Check for player in detection radius
            Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

            if (playerInRange != null)
            {
                Fire();
            }
        }
    }

    private IEnumerator FireWithDelay()
    {
        canFire = false; // Prevent multiple firings
        yield return new WaitForSeconds(fireDelay);
        FireCatapult();
    }

    public void Fire()
    {
        StartCoroutine(FireWithDelay());
    }

    public void FireCatapult()
    {
        foreach (GameObject cargo in CargoLoad)
        {
            if (cargo != null)
            {
                if (cargo.layer == LayerMask.NameToLayer("Enemy"))
                {
                    if (GameObject.Find("EnemySpawner").transform)
                    {
                        cargo.transform.parent = GameObject.Find("EnemySpawner").transform;
                        Debug.LogError("setting parent to EnemySpawner");
                    }
                    else
                    {
                        Debug.LogError("EnemySpawner object not found.");
                    }
                }
                else
                {
                    if (GameObject.Find("PlayerContainer").transform)
                        cargo.transform.parent = GameObject.Find("PlayerContainer").transform;
                }

                cargo.GetComponent<Collider2D>().enabled = true;
                cargo.GetComponent<Rigidbody2D>().simulated = true;
                Rigidbody2D rb = cargo.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Apply force in the direction of the cargo's right vector
                    rb.AddForce(cargo.transform.right * launchForce);
                }
                else
                {
                    Debug.LogError("CargoLoad object does not have a Rigidbody2D component.");
                }

                // Enable all components of the cargo object
                foreach (var component in cargo.GetComponents<Component>())
                {
                    if (component is Behaviour behaviour)
                    {
                        behaviour.enabled = true;
                    }

                   
                }
            }
        }

        this.enabled = false; // Disable the script after firing
    }

    // Optional: Visualize detection radius in scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}