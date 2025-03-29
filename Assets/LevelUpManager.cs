using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class LevelUpManager : MonoBehaviour
{
    public GameObject levelUpUI;
    public float moveDuration = 2f;
    public float waitTime = 2f;
    public AnimationCurve moveCurve;

    private float elapsedTime = 0f;
    private bool isLevelingUp = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    [Header("Level Settings")]
    public List<int> levels; // List of levels with integer values set via inspector
    public int CurrentLevelIndex { get; private set; } = 0; // Public property with private setter

    private TextMeshProUGUI oldLevelText;
    private TextMeshProUGUI newLevelText;

    private void Start()
    {
        // Subscribe to the OnXPEarned event from the PlayerXP script
        PlayerXP playerXP = (PlayerXP)FindAnyObjectByType(typeof(PlayerXP));
        if (playerXP != null)
        {
            playerXP.OnXPEarned += OnXPEarned;
        }

        // Get references to the TextMeshPro components
        oldLevelText = levelUpUI.transform.Find("OldLevel").GetComponent<TextMeshProUGUI>();
        newLevelText = levelUpUI.transform.Find("NewLevel").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (isLevelingUp)
        {
            MoveLevelUpUI();
        }
    }

    private void OnXPEarned(int currentXP)
    {
        // Check if the next level is reached
        if (CurrentLevelIndex < levels.Count && currentXP >= levels[CurrentLevelIndex])
        {
            CurrentLevelIndex++;
            TriggerLevelUp();
        }
    }

    public void TriggerLevelUp()
    {
        isLevelingUp = true;
        levelUpUI.SetActive(true);
        startPosition = levelUpUI.transform.position;
        targetPosition = new Vector3(startPosition.x, Screen.height + levelUpUI.GetComponent<RectTransform>().rect.height, startPosition.z);
        elapsedTime = -waitTime; // Start with negative time to account for waiting

        // Update the text on OldLevel and NewLevel
        oldLevelText.text = (CurrentLevelIndex - 1).ToString();
        newLevelText.text = CurrentLevelIndex.ToString();
    }

    private void MoveLevelUpUI()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 0)
        {
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            float curvedT = moveCurve.Evaluate(t);
            levelUpUI.transform.position = Vector3.Lerp(startPosition, targetPosition, curvedT);

            if (t >= 1f)
            {
                CompleteLevelUp();
            }
        }
    }

    private void CompleteLevelUp()
    {
        isLevelingUp = false;
        levelUpUI.SetActive(false);
        levelUpUI.transform.localPosition = Vector3.zero; // Reset position for next use
    }
}
