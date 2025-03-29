// PlayerInventory.cs
using System.Collections.Generic;
using TheProphecy.Items;
using TheProphecy.Player;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<GameObject> collectedItems = new List<GameObject>();
    [SerializeField] private GameObject toasterMessagePrefab; // Prefab for the toaster message

    private Queue<(Sprite, string)> toasterQueue = new Queue<(Sprite, string)>();
    private bool isToasterActive = false;
    private BasePlayer player; // Cache the BasePlayer reference

    void Start()
    {
        player = GetComponent<BasePlayer>(); // Get the BasePlayer component
        if (player == null)
        {
            Debug.LogError("BasePlayer component not found on this GameObject.");
        }
    }

    public void AddItem(GameObject item)
    {
        collectedItems.Add(item);
        Debug.Log("Item added to inventory: " + item.name);

        LootItem lootItem = item.GetComponent<LootItem>();
        if (lootItem != null)
        {
            // Get the sprite from the SpriteRenderer component
            Sprite itemSprite = item.GetComponent<SpriteRenderer>().sprite;

            // Queue the toaster message
            toasterQueue.Enqueue((itemSprite, lootItem.itemName));
            if (!isToasterActive)
            {
                ShowNextToasterMessage();
            }

            // Apply the item effect
            CheckLootItemEffect(lootItem);
        }
    }

    private void CheckLootItemEffect(LootItem lootItem)
    {
        // Check if the loot item has an effect
        if (lootItem.itemType == LootItem.ItemType.Health)
        {
            player.GiveHealth(lootItem.itemValue);
            Debug.Log("Health added: " + lootItem.itemValue);
        }

        if (lootItem.itemType == LootItem.ItemType.Shield)
        {
            player.GiveShield(lootItem.itemValue);
            Debug.Log($"Shield added: {lootItem.itemValue}. New Shield Value: {player.shield}");
        }
    }

    public List<GameObject> GetCollectedItems()
    {
        return collectedItems;
    }

    private void ShowNextToasterMessage()
    {
        if (toasterQueue.Count > 0)
        {
            var (itemSprite, itemName) = toasterQueue.Dequeue();
            GameObject toasterMessageObj = Instantiate(toasterMessagePrefab, GameObject.FindObjectOfType<Canvas>().transform);
            ToasterMessage toasterMessageScript = toasterMessageObj.GetComponent<ToasterMessage>();
            isToasterActive = true;
            toasterMessageScript.OnToasterComplete += ResetToaster;
            toasterMessageScript.ShowMessage(itemSprite, itemName);
        }
    }

    private void ResetToaster()
    {
        isToasterActive = false;
        ShowNextToasterMessage();
    }
}
