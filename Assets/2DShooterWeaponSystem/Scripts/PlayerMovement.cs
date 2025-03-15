using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/// <summary>
/// Very basic, example player movement script for demo scenes.
/// </summary>
/// 
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour {

    public enum PlayerMovementType
    {
        Normal,
        FreeAim
    }

    /// <summary>
    /// Sets the player movement type - either normal top down horizontal or vertical (good for shmups), or to FreeAim, which allows for free aiming type top down shooter style controls.
    /// </summary>
    public PlayerMovementType playerMovementType;

    /// <summary>
    /// Reference to a light object used as an example flash light in some demo scenes for the player.
    /// </summary>
    public Light FlashLight;

    /// <summary>
    /// Flag if the player is moving or not.
    /// </summary>
    public bool IsMoving;

    /// <summary>
    /// Movement speed
    /// </summary>
    [Range(1f, 5f)]
    public float freeAimMovementSpeed = 2f;

    private Animator playerAnimator;

    private Rigidbody2D rb;

    private Vector2 movementInput;

    private void Awake()
    {
        var input = GetComponent<PlayerInput>();
        if (input != null)
        {
            input.onActionTriggered += HandleActions;
        }

        rb = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start () {
        // If the player has an animator component then get a handle to it and cache it in the playerAnimator field.
	    if (gameObject.GetComponent<Animator>() != null)
	    {
	        playerAnimator = gameObject.GetComponent<Animator>();
	    }
	}

    private void HandleActions(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Move":
                if (context.phase == InputActionPhase.Performed)
                {
                    var v = context.ReadValue<Vector2>();
                    IsMoving = true;
                    movementInput = v;
                }
                else
                {
                    IsMoving = false;
                    movementInput = Vector2.zero;
                }
                break;
        }
    }

	// Update is called once per frame
	void Update () 
    {
        // Clamp the player to the screen
        var pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.05f, 0.95f);
        pos.y = Mathf.Clamp(pos.y, 0.05f, 0.95f);
        transform.position = Camera.main.ViewportToWorldPoint(pos);

        if (playerAnimator != null)
        {
            if (IsMoving)
            {
                playerAnimator.SetBool("IsMoving", true);
            }
            else
            {
                playerAnimator.SetBool("IsMoving", false);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 movement = movementInput * freeAimMovementSpeed * Time.fixedDeltaTime;
        switch (playerMovementType)
        {
            // Normal top down horizontal or vertical style player controls
            case PlayerMovementType.Normal:
                rb.MovePosition(rb.position + movement);
                break;

            // top down free aim style player controls
            case PlayerMovementType.FreeAim:
                rb.MovePosition(rb.position + movement);
                break;
        }
    }
}
