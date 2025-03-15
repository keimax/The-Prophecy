using TheProphecy.Player;
using UnityEngine;

namespace TheProphecy.Interfaces
{
    public interface IMovement
    {
        void HandleMovement(Transform transform, VirtualJoystick joystickInput, Rigidbody2D rigidbody, MovementController.MovementSettings settings, float rotationOffset, ref Vector2 currentVelocity, ref float currentRotationSpeed);
    }
}