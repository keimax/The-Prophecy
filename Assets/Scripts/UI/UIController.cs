using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TheProphecy.LevelRun;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _inGameUI;
    [SerializeField] private GameObject _deathScreenUI;
    [SerializeField] private GameObject _WinScreenUI;
    [SerializeField] public GameObject _JoystickLeftUI;

    private EventSystem _eventSystem;
    private List<Selectable> _uiElements = new List<Selectable>();
    private int _currentIndex = 0;

    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _clickAction;
    private InputAction _clickSettingsAction;

    public event Action OnUIClicked;
    public event Action<Vector2> OnMoveInput;

    private Color _defaultColor = Color.white;
    [SerializeField] public Color _selectedColor = Color.red;

    private bool _canMove = true;

    private void Awake()
    {
        _eventSystem = EventSystem.current;
        _playerInput = GetComponent<PlayerInput>();

        if (_playerInput != null)
        {
            _moveAction = _playerInput.actions["Move"];
            _clickAction = _playerInput.actions["UIClick"];
            _clickSettingsAction = _playerInput.actions["SettingsClick"];
        }

        RefreshUIElements();
    }

    private void OnEnable()
    {
        if (_moveAction != null)
            _moveAction.performed += OnMovePerformed;

        if (_clickAction != null)
            _clickAction.performed += OnUIClick;

        if (_clickSettingsAction != null)
            _clickSettingsAction.performed += OnSettingsClick;
    }

    private void OnDisable()
    {
        if (_moveAction != null)
            _moveAction.performed -= OnMovePerformed;

        if (_clickAction != null)
            _clickAction.performed -= OnUIClick;

        if (_clickSettingsAction != null)
            _clickSettingsAction.performed -= OnSettingsClick;
    }

    public void RefreshUIElements()
    {
        CollectUIElements();
        EnsureValidSelection();
    }

    private void CollectUIElements()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        _uiElements.Clear();

        foreach (Button btn in buttons)
        {
            if (btn.gameObject.activeInHierarchy)
                _uiElements.Add(btn);
        }

        if (_uiElements.Count > 0)
        {
            _currentIndex = Mathf.Clamp(_currentIndex, 0, _uiElements.Count - 1);
            FocusButton(_uiElements[_currentIndex]);
        }
        else
        {
            Debug.LogWarning("No UI Buttons found!");
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (!_canMove)
            return;

        Vector2 moveInput = context.ReadValue<Vector2>();
        OnMoveInput?.Invoke(moveInput);

        if (moveInput == Vector2.zero)
            return;

        MoveSelection(moveInput);
        _canMove = false;
        StartCoroutine(ResetMoveCooldown());
    }

    private IEnumerator ResetMoveCooldown()
    {
        yield return new WaitForSeconds(0.25f);
        _canMove = true;
    }

    private void MoveSelection(Vector2 direction)
    {
        if (_uiElements.Count == 0)
            return;

        Selectable current = _uiElements[_currentIndex];
        Vector3 currentPos = current.transform.position;

        float bestDistance = Mathf.Infinity;
        Selectable bestCandidate = null;

        foreach (Selectable candidate in _uiElements)
        {
            if (candidate == current || !candidate.gameObject.activeInHierarchy)
                continue;

            Vector3 candidatePos = candidate.transform.position;
            Vector3 diff = candidatePos - currentPos;
            float dot = Vector2.Dot(diff.normalized, direction);
            if (dot <= 0)
                continue;

            float distance = diff.magnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestCandidate = candidate;
            }
        }

        if (bestCandidate != null)
        {
            _currentIndex = _uiElements.IndexOf(bestCandidate);
            FocusButton(bestCandidate);
        }
        else
        {
            Debug.Log("No candidate found in the given direction.");
        }
    }

    private void FocusButton(Selectable button)
    {
        if (button == null)
            return;

        foreach (Selectable btn in _uiElements)
            SetButtonColor(btn, _defaultColor);

        SetButtonColor(button, _selectedColor);
        _eventSystem.SetSelectedGameObject(button.gameObject);
        Debug.Log($"UI Element Selected: {button.gameObject.name}");
    }

    private void SetButtonColor(Selectable button, Color color)
    {
        if (button.TryGetComponent(out Image image))
            image.color = color;
    }



    private void OnSettingsClick(InputAction.CallbackContext context)
    {
        Debug.Log("Settings action performed!");
        ToggleInventoryScreen(!_inventoryUI.activeSelf);
    }


    private void OnUIClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log("UIClick action performed!");

            // Get mouse position
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            PointerEventData pointerData = new PointerEventData(_eventSystem)
            {
                position = mousePosition
            };

            // Raycast to check which UI element is under the mouse
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // Find the first UI element clicked
            foreach (var result in results)
            {
                Button clickedButton = result.gameObject.GetComponent<Button>();
                if (clickedButton != null)
                {
                    // Update current index and focus the button
                    _currentIndex = _uiElements.IndexOf(clickedButton);
                    FocusButton(clickedButton);
                    clickedButton.onClick.Invoke(); // Invoke the button click
                    OnUIClicked?.Invoke();
                    return; // Exit after handling the click
                }
            }

            Debug.LogWarning("No valid UI element clicked!");
        }
    }

    public void OnPlayAgainButtonPressed()
    {
        Debug.Log("Play Again Pressed!");
        LevelManager.instance.ResetLevel();
    }

    public void ToggleDeathScreen(bool toggle)
    {
        _deathScreenUI.SetActive(toggle);
        RefreshUIElements();
    }

    public void ToggleWinScreen(bool toggle)
    {
        _WinScreenUI.SetActive(toggle);
        RefreshUIElements();
    }

    public void ToggleInventoryScreen(bool toggle)
    {
        _inventoryUI.SetActive(toggle);
        RefreshUIElements();
    }


    private void EnsureValidSelection()
    {
        if (_uiElements.Count == 0)
            return;

        if (_currentIndex >= _uiElements.Count)
            _currentIndex = 0;

        FocusButton(_uiElements[_currentIndex]);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game Pressed!");
        Application.Quit();
    }
}