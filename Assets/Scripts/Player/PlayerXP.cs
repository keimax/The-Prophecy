using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    // Public variable to store the current XP
    public int currentXP { get; private set; }

    // Event to be fired whenever XP is increased
    public event System.Action<int> OnXPEarned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentXP = 0; // Initialize current XP to 0
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Function to earn XP, can be accessed from other scripts
    public void EarnXP(int amount)
    {
        currentXP += amount;
        OnXPEarned?.Invoke(currentXP); // Fire the event
    }
}
