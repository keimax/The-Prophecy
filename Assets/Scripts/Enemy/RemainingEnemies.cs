using UnityEngine;

using TheProphecy;


public class RemainingEnemies : MonoBehaviour
{
    [SerializeField] private UIController _uIController;
    public Transform enemyContainer;

    private void Start()
    {
        InvokeRepeating(nameof(CountEnemies), 0f, 2f);
    }

    public void CountEnemies()
    {
        if (enemyContainer.childCount <= 0)
        {
            _uIController.ToggleWinScreen(true);
        }
    }
}
