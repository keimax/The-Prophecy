using UnityEngine;
using TheProphecy.Interfaces;

namespace TheProphecy.Player
{
    public class MovementController : MonoBehaviour, ISkill
    {
        [Header("References")]
        [SerializeField] private VirtualJoystick _moveJoystick;
        private Rigidbody2D _rigidbody;
        private IMovement currentMovement;

        [System.Serializable]
        public class MovementSettings
        {
            [Range(0f, 20f)]
            public float maxForwardSpeed = 10f; // Maximum forward speed

            [Range(0f, 1200f)]
            public float maxTurnSpeed = 100f; // Maximum turn speed (degrees per second)

            [Range(0f, 3500f)]
            public float turnAcceleration = 50f; // Acceleration rate for turning

            [Range(0f, 50f)]
            public float forwardAcceleration = 5f; // Acceleration rate for forward movement

            [Range(0f, 1f)]
            public float linearDamping = 0.1f; // Damping to reduce jitter

            [Range(0f, 5f)]
            public float rotationDamping = 0.1f; // Damping for rotation (not used in current logic)

            [Range(0f, 500f)]
            public float rotationDeceleration = 500f; // How quickly the rotation slows down

            [Range(0f, 100f)]
            public float forwardDeceleration = 20f; // How quickly the forward speed slows down

            [Range(0f, 1f)]
            public float forwardDamping = 0.1f; // New damping factor for forward speed
        }

        [Header("Movement Settings for Touch")]
        public MovementSettings touchSettings;

        [Header("Movement Settings for Mouse")]
        public MovementSettings mouseSettings;

        [Header("Movement Settings for Gamepad")]
        public MovementSettings gamepadSettings;

        [Header("Movement Settings for Keyboard")]
        public MovementSettings keyboardSettings;

        [Header("Movement Settings for Touch Alternative")] // New header for TouchAlternative settings
        public MovementSettings touchAlternativeSettings; // New settings variable

        [Header("Rotation Offset")]
        [Range(-360f, 360f)]
        public float rotationOffset = 90f; // Offset to correct ship direction

        private Vector2 _currentVelocity = Vector2.zero;
        private float _currentRotationSpeed = 0f; // Current rotation speed

        private void Start()
        {
            Application.targetFrameRate = 120; // Set FPS to 120
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.linearDamping = touchSettings.linearDamping; // Set linear damping
            _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate; // Enable interpolation

            // Initialize the appropriate movement class
            InitializeMovement();
        }

        private void InitializeMovement()
        {
            switch (_moveJoystick.preferredInputType)
            {
                case GameManager.InputType.Mouse:
                    currentMovement = new MouseMovement();
                    break;
                case GameManager.InputType.Keyboard:
                    currentMovement = new KeyboardMovement();
                    break;
                case GameManager.InputType.Gamepad:
                    currentMovement = new GamepadMovement();
                    break;
                case GameManager.InputType.Touch:
                    currentMovement = new TouchMovement();
                    break;
                case GameManager.InputType.TouchAlternative: // New case for alternative touch movement
                    currentMovement = new TouchMovementAlternative();
                    break;
            }
        }

        private void FixedUpdate()
        {
            currentMovement.HandleMovement(transform, _moveJoystick, _rigidbody, GetCurrentSettings(), rotationOffset, ref _currentVelocity, ref _currentRotationSpeed);
        }

        private MovementSettings GetCurrentSettings()
        {
            switch (_moveJoystick.preferredInputType)
            {
                case GameManager.InputType.Mouse:
                    return mouseSettings;
                case GameManager.InputType.Keyboard:
                    return keyboardSettings;
                case GameManager.InputType.Gamepad:
                    return gamepadSettings;
                case GameManager.InputType.Touch:
                    return touchSettings;
                case GameManager.InputType.TouchAlternative: // Return settings for TouchAlternative
                    return touchAlternativeSettings; // New return for alternative touch settings
                default:
                    return touchSettings; // Fallback to touch settings
            }
        }

        // Implementing the ISkill interface
        public float GetCooldownPercentage()
        {
            return 1f; // Represents no cooldown
        }
    }
}