using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyList
{
    [HideInInspector]
    public bool ignoreFiringArc = true;
    private float distanceRange;
    private Transform target;
    private string tag;
    private List<GameObject> previousEnemiesInRange = new List<GameObject>();
    private List<GameObject> enemiesInRange = new List<GameObject>();
    public List<GameObject> EnemiesInRange => enemiesInRange; // Expose a read-only version of the list
    public event Action<List<GameObject>> OnEnemyListChanged;

    public EnemyList(float distanceRange, Transform target, string tag, bool ignoreFiring)
    {
        this.distanceRange = distanceRange;
    //    Debug.Log(distanceRange);
        this.target = target;
        this.tag = tag;
        this.ignoreFiringArc = ignoreFiring;
        previousEnemiesInRange = new List<GameObject>(enemiesInRange);
    }


    public void SetEnemyList(List<GameObject> L)
    {
        OnEnemyListChanged?.Invoke(L);
    }


    public void UpdateEnemyList()
    {
        enemiesInRange.Clear();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(target.transform.position, distanceRange);



        // Sort the enemies based on their distance from the target
        enemiesInRange.Sort((a, b) => Vector2.Distance(a.transform.position, target.position).CompareTo(Vector2.Distance(b.transform.position, target.position)));

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag(tag)) // ccheck if the found colliders fit our tag that we searching for
            {
                // only colliders from enemy layer and player layer can be added to the list (e.g. no bullets)
                if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy") || collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                    enemiesInRange.Add(collider.gameObject);
            }
        }

        // Compare the previousEnemiesInRange with the updated enemiesInRange
        if (!ListsAreEqual(enemiesInRange, previousEnemiesInRange))
        {
            if (ignoreFiringArc) // shoot everything in range!
            {
                Debug.Log("invoke enemy list ");
                OnEnemyListChanged?.Invoke(enemiesInRange);
            }
        }

        // Update the previousEnemiesInRange with the current enemiesInRange
        previousEnemiesInRange = new List<GameObject>(enemiesInRange);
    }

    // Helper method to check if two lists contain the same elements (order doesn't matter)
    private bool ListsAreEqual(List<GameObject> list1, List<GameObject> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }

        HashSet<GameObject> set1 = new HashSet<GameObject>(list1);
        HashSet<GameObject> set2 = new HashSet<GameObject>(list2);

        return set1.SetEquals(set2);
    }
}
