using System.Collections.Generic;
using UnityEngine;

public class LootDropManager : MonoBehaviour
{
    [System.Serializable]
    public class LootCategory
    {
        [HideInInspector] public string name;
        public List<GameObject> items;
        [Range(0, 100)] public float probability; // Probability of selecting this category
    }

    public LootCategory basicLoot = new LootCategory { name = "Basic Loot" };
    public LootCategory rareLoot = new LootCategory { name = "Rare Loot" };
    public LootCategory epicLoot = new LootCategory { name = "Epic Loot" };
    public LootCategory legendaryLoot = new LootCategory { name = "Legendary Loot" };
    public LootCategory specialLoot = new LootCategory { name = "Special Loot" };

    [Header("Overall Drop Chances (Must sum to 100%)")]
    [Range(0, 100)] public float noLootChance = 50;
    [Range(0, 100)] public float oneItemChance = 30;
    [Range(0, 100)] public float twoItemChance = 15;
    [Range(0, 100)] public float threeItemChance = 5;

    [Header("Loot Drop Force")]
    [Range(1f, 10f)] public float minForce = 1f;
    [Range(1f, 10f)] public float maxForce = 3f;

    private void OnValidate()
    {
        AdjustProbabilities();
        AdjustDropChances();
        NormalizeProbabilities();
        NormalizeDropChances();
    }

    private void AdjustProbabilities()
    {
        List<LootCategory> lootCategories = new List<LootCategory> { basicLoot, rareLoot, epicLoot, legendaryLoot, specialLoot };
        float totalProbability = 0;
        foreach (var category in lootCategories)
        {
            totalProbability += category.probability;
        }

        if (totalProbability > 100)
        {
            float excess = totalProbability - 100;
            foreach (var category in lootCategories)
            {
                category.probability -= (category.probability / totalProbability) * excess;
            }
        }

        // Round probabilities to two decimal places
        foreach (var category in lootCategories)
        {
            category.probability = Mathf.Round(category.probability * 100f) / 100f;
        }
    }

    private void AdjustDropChances()
    {
        float totalDropChance = noLootChance + oneItemChance + twoItemChance + threeItemChance;

        if (totalDropChance > 100)
        {
            float excess = totalDropChance - 100;
            noLootChance -= (noLootChance / totalDropChance) * excess;
            oneItemChance -= (oneItemChance / totalDropChance) * excess;
            twoItemChance -= (twoItemChance / totalDropChance) * excess;
            threeItemChance -= (threeItemChance / totalDropChance) * excess;
        }

        // Round drop chances to two decimal places
        noLootChance = Mathf.Round(noLootChance * 100f) / 100f;
        oneItemChance = Mathf.Round(oneItemChance * 100f) / 100f;
        twoItemChance = Mathf.Round(twoItemChance * 100f) / 100f;
        threeItemChance = Mathf.Round(threeItemChance * 100f) / 100f;
    }

    private void NormalizeProbabilities()
    {
        List<LootCategory> lootCategories = new List<LootCategory> { basicLoot, rareLoot, epicLoot, legendaryLoot, specialLoot };
        float totalProbability = 0;
        foreach (var category in lootCategories)
        {
            totalProbability += category.probability;
        }

        if (totalProbability != 0)
        {
            foreach (var category in lootCategories)
            {
                category.probability = (category.probability / totalProbability) * 100;
            }
        }
    }

    private void NormalizeDropChances()
    {
        float totalDropChance = noLootChance + oneItemChance + twoItemChance + threeItemChance;

        if (totalDropChance != 0)
        {
            noLootChance = (noLootChance / totalDropChance) * 100;
            oneItemChance = (oneItemChance / totalDropChance) * 100;
            twoItemChance = (twoItemChance / totalDropChance) * 100;
            threeItemChance = (threeItemChance / totalDropChance) * 100;
        }
    }

    public void DropLoot(Transform enemyTransform, Vector2 enemyDirection)
    {
        float roll = Random.Range(0f, 100f);
        int itemCount = DetermineLootCount(roll);

        for (int i = 0; i < itemCount; i++)
        {
            GameObject lootItem = GetRandomLoot();
            if (lootItem != null)
            {
                SpawnLoot(lootItem, enemyTransform, enemyDirection);
            }
        }
    }

    private int DetermineLootCount(float roll)
    {
        if (roll < noLootChance) return 0;
        if (roll < noLootChance + oneItemChance) return 1;
        if (roll < noLootChance + oneItemChance + twoItemChance) return 2;
        return 3;
    }

    private GameObject GetRandomLoot()
    {
        List<LootCategory> lootCategories = new List<LootCategory> { basicLoot, rareLoot, epicLoot, legendaryLoot, specialLoot };
        float totalProbability = 0;
        foreach (var category in lootCategories)
        {
            totalProbability += category.probability;
        }

        float roll = Random.Range(0f, totalProbability);
        float cumulative = 0;

        foreach (var category in lootCategories)
        {
            cumulative += category.probability;
            if (roll <= cumulative && category.items.Count > 0)
            {
                return category.items[Random.Range(0, category.items.Count)];
            }
        }
        return null;
    }

    private void SpawnLoot(GameObject lootItem, Transform enemyTransform, Vector2 enemyDirection)
    {
        Vector2 spawnPosition = (Vector2)enemyTransform.position - enemyDirection.normalized * 0.5f;
        GameObject spawnedLoot = Instantiate(lootItem, spawnPosition, Quaternion.identity);

        Rigidbody2D rb = spawnedLoot.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomForce = Random.insideUnitCircle.normalized * Random.Range(minForce, maxForce);
            rb.AddForce(randomForce, ForceMode2D.Impulse);
        }
    }
}