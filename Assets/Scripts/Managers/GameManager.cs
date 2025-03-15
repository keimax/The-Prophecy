using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Player object
    [SerializeField] private GameObject player;
    public GameObject Player { get => player; set => player = value; }

    // Player ID
    [SerializeField] private int player_ID;
    public int Player_ID { get => player_ID; set => player_ID = value; }

    // Number of other human players currently in the game
    [SerializeField] private int humanPlayersCount;
    public int HumanPlayersCount { get => humanPlayersCount; set => humanPlayersCount = value; }

    // List of human player game objects
    [SerializeField] private List<GameObject> humanPlayers = new List<GameObject>();
    public List<GameObject> HumanPlayers { get => humanPlayers; set => humanPlayers = value; }

    // Statistics
    [SerializeField] private int enemiesKilledByPlayer;
    public int EnemiesKilledByPlayer { get => enemiesKilledByPlayer; set => enemiesKilledByPlayer = value; }

    [SerializeField] private int activeEnemiesCount;
    public int ActiveEnemiesCount { get => activeEnemiesCount; set => activeEnemiesCount = value; }

    [SerializeField] private float gameTime;
    public float GameTime { get => gameTime; set => gameTime = value; }

    // List of level game objects
    [SerializeField] private List<GameObject> levels = new List<GameObject>();
    public List<GameObject> Levels { get => levels; set => levels = value; }

    // Current active level
    [SerializeField] private GameObject currentActiveLevel;
    public GameObject CurrentActiveLevel { get => currentActiveLevel; set => currentActiveLevel = value; }

    // Current input control enum
    public enum InputType
    {
        Touch,
        Mouse,
        Keyboard,
        Gamepad,
        TouchAlternative
    }
    [SerializeField] private InputType currentInputControl;
    public InputType CurrentInputControl { get => currentInputControl; set => currentInputControl = value; }

    // References to containers and UIController
    [SerializeField] private GameObject playerContainer;
    public GameObject PlayerContainer { get => playerContainer; set => playerContainer = value; }

    [SerializeField] private GameObject enemyContainer;
    public GameObject EnemyContainer { get => enemyContainer; set => enemyContainer = value; }

    [SerializeField] private GameObject projectileContainer;
    public GameObject ProjectileContainer { get => projectileContainer; set => projectileContainer = value; }

    [SerializeField] private UIController uiController;
    public UIController UIController { get => uiController; set => uiController = value; }

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize game time
        GameTime = 0f;
        UpdateJoystickInputType();
    }

    private void Update()
    {
        // Update game time
        GameTime += Time.deltaTime;
    }

    private void OnValidate()
    {
        // Update joystick input type when the InputType is changed in the inspector
        UpdateJoystickInputType();
    }

    private void UpdateJoystickInputType()
    {
        if (uiController != null && uiController._JoystickLeftUI != null)
        {
            var joystick = uiController._JoystickLeftUI.GetComponent<VirtualJoystick>();
            if (joystick != null)
            {
                joystick.preferredInputType = currentInputControl;
            }
        }
    }

    // ... (rest of your methods)
}