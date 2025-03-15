using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ObjectComponentToggler : MonoBehaviour
{
    [System.Serializable]
    public class ToggleItem
    {
        public GameObject targetObject;
        public Component targetComponent;
        public float delay = 0f;
        public bool enable = true;
    }

    [Header("Objects and Components to Toggle")]
    public List<ToggleItem> itemsToToggle = new List<ToggleItem>();

    private void Start()
    {
        foreach (var item in itemsToToggle)
        {
            StartCoroutine(ToggleAfterDelay(item));
        }
    }

    private IEnumerator ToggleAfterDelay(ToggleItem item)
    {
        yield return new WaitForSeconds(item.delay);

        // If a specific component is set, toggle that
        if (item.targetComponent != null)
        {
            Behaviour componentBehaviour = item.targetComponent as Behaviour;
            if (componentBehaviour != null)
            {
                componentBehaviour.enabled = item.enable;
            }
        }

        // If a GameObject is set, toggle its active state
        if (item.targetObject != null)
        {
            item.targetObject.SetActive(item.enable);
        }
    }
}