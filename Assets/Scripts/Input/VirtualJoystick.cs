using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour
{
    //public enum InputType
    //{
    //    Touch,
    //    Mouse,
    //    Keyboard,
    //    Gamepad,
    //    TouchAlternative
    //}

    [Header("Input Settings")]
    public GameManager.InputType preferredInputType = GameManager.InputType.Touch; // Dropdown in Inspector

    public RectTransform joystickBackgroundTransform;
    public RectTransform joystickHandleTransform;
    public float joystickMovementLimit = 100f;
    public PlayerInput playerInputComponent;

    private InputAction playerMoveAction;
    private InputAction mouseClickAction;
    private InputAction playerMoveKeysAction;
    private InputAction playerMoveTouchAction; // Touch input action
    private bool isDragging = false;

    [SerializeField] private bool enableDebugLogs = true; // Set to true for debug logs

    private Vector2 input = Vector2.zero;

    void Awake()
    {
        if (playerInputComponent == null)
        {
            Debug.LogError("PlayerInput component not assigned!");
            enabled = false;
            return;
        }

        playerMoveAction = playerInputComponent.actions.FindAction("PlayerMove");
        mouseClickAction = playerInputComponent.actions.FindActionMap("Player").FindAction("MouseClick");
        playerMoveKeysAction = playerInputComponent.actions.FindAction("PlayerMoveKeys");
        playerMoveTouchAction = playerInputComponent.actions.FindAction("PlayerMoveTouch");

        if (joystickBackgroundTransform == null || joystickHandleTransform == null)
        {
            Debug.LogError("Joystick Background or Handle not assigned!");
            enabled = false;
            return;
        }

        if (joystickHandleTransform.parent != joystickBackgroundTransform)
        {
            Debug.LogError("Joystick Handle must be a child of Joystick Background!");
            enabled = false;
            return;
        }

        joystickHandleTransform.anchoredPosition = Vector2.zero;
    }

    void Start()
    {
        playerMoveAction.Enable();
        mouseClickAction.Enable();
        playerMoveKeysAction.Enable();
        playerMoveTouchAction.Enable(); // Enable touch action
    }

    void Update()
    {
        input = Vector2.zero;
        HandleInput();
        UpdateHandlePosition();
    }

    private void HandleInput()
    {
        switch (preferredInputType)
        {
            case GameManager.InputType.Touch:
                HandleTouchInput();
                break;
            case GameManager.InputType.TouchAlternative: // Treat TouchAlternative as Touch
                HandleTouchInput();
                break;
            case GameManager.InputType.Mouse:
                HandleMouseInput();
                break;
            case GameManager.InputType.Keyboard:
                HandleKeyboardInput();
                break;
            case GameManager.InputType.Gamepad:
                HandleGamepadInput();
                break;
        }
    }

    private void HandleTouchInput()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            if (IsTouchWithinJoystickArea(touchPosition))
            {
                ProcessTouchInput(touchPosition);
            }
        }
        else
        {
            isDragging = false;
        }
    }

    private void HandleMouseInput()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            if (IsTouchWithinJoystickArea(mousePosition))
            {
                ProcessMouseInput(mousePosition);
            }
        }
        else
        {
            isDragging = false;
        }
    }

    private void HandleKeyboardInput()
    {
        // Using WASD for movement
        Vector2 keyboardInput = new Vector2(
            Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0,
            Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0
        );
        input = keyboardInput.normalized;
        isDragging = input.magnitude > 0;

        if (enableDebugLogs)
        {
            Debug.Log($"Keyboard Input: {input}");
        }
    }

    private void HandleGamepadInput()
    {
        if (Gamepad.current != null)
        {
            Vector2 gamepadInput = Gamepad.current.leftStick.ReadValue();
            input = gamepadInput;
            isDragging = input.magnitude > 0;

            if (enableDebugLogs)
            {
                Debug.Log($"Gamepad Input: {input}");
            }
        }
    }

    private void ProcessTouchInput(Vector2 inputPosition)
    {
        Vector2 localInputPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackgroundTransform,
            inputPosition,
            joystickBackgroundTransform.GetComponentInParent<Canvas>().worldCamera,
            out localInputPosition
        );

        input.x = localInputPosition.x / joystickMovementLimit;
        input.y = localInputPosition.y / joystickMovementLimit;
        input = Vector2.ClampMagnitude(input, 1f);

        if (enableDebugLogs)
        {
            Debug.Log($"Touch Input Vector (Processed): {input}");
        }

        isDragging = true; // Set dragging to true while touch is active
    }

    private void ProcessMouseInput(Vector2 inputPosition)
    {
        Vector2 localInputPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackgroundTransform,
            inputPosition,
            joystickBackgroundTransform.GetComponentInParent<Canvas>().worldCamera,
            out localInputPosition
        );

        input.x = localInputPosition.x / joystickMovementLimit;
        input.y = localInputPosition.y / joystickMovementLimit;
        input = Vector2.ClampMagnitude(input, 1f);

        if (enableDebugLogs)
        {
            Debug.Log($"Mouse Input Vector (Processed): {input}");
        }

        isDragging = true; // Set dragging to true while mouse is pressed
    }

    private void UpdateHandlePosition()
    {
        Vector2 newHandlePosition = input * joystickMovementLimit;
        joystickHandleTransform.anchoredPosition = newHandlePosition;
    }

    public Vector2 GetJoystickInput()
    {
        return input;
    }

    public Vector2 Direction => input; // Add this property to get the joystick direction

    private bool IsTouchWithinJoystickArea(Vector2 touchPosition)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackgroundTransform,
            touchPosition,
            joystickBackgroundTransform.GetComponentInParent<Canvas>().worldCamera,
            out localPoint
        );

        // Create a larger rectangle for the joystick area, adjust the multiplier as needed
        float expansionFactor = 4f; // Adjust this factor to increase the touchable area
        Rect expandedRect = new Rect(
            -joystickMovementLimit * expansionFactor,  // Adjusted for expansion
            -joystickMovementLimit * expansionFactor,  // Adjusted for expansion
            joystickMovementLimit * 2 * expansionFactor, // Width expanded evenly
            joystickMovementLimit * 2 * expansionFactor  // Height expanded evenly
        );

        // Check if the localPoint is within the bounds of the expanded joystick background
        return expandedRect.Contains(localPoint);
    }

    private void OnEnable()
    {
        playerMoveAction?.Enable();
        mouseClickAction?.Enable();
        playerMoveKeysAction?.Enable();
        playerMoveTouchAction?.Enable(); // Enable touch action
    }

    private void OnDisable()
    {
        playerMoveAction?.Disable();
        mouseClickAction?.Disable();
        playerMoveKeysAction?.Disable();
        playerMoveTouchAction?.Disable(); // Disable touch action
    }
}