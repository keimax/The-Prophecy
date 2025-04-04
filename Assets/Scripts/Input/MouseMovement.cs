using TheProphecy.Player;
using UnityEngine;
using TheProphecy.Interfaces; 

public class MouseMovement : IMovement
{
    public void HandleMovement(Transform transform, VirtualJoystick joystickInput, Rigidbody2D rigidbody, MovementController.MovementSettings settings, float rotationOffset, ref Vector2 currentVelocity, ref float currentRotationSpeed)
    {
        Vector2 joystickInputValue = joystickInput.GetJoystickInput();

        // Move forward in the direction the ship is facing based on joystick input
        if (joystickInputValue.y > 0)
        {
            currentVelocity = transform.up * settings.maxForwardSpeed;
        }
        else if (joystickInputValue.y < 0)
        {
            currentVelocity = Vector2.zero; // Prevent backward movement
        }

        // Rotate based on joystick input (inverted)
        if (joystickInputValue.x != 0)
        {
            float targetAngle = Mathf.Atan2(joystickInputValue.y, joystickInputValue.x) * Mathf.Rad2Deg + rotationOffset;
            float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle);
            currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, -angleDifference * settings.maxTurnSpeed, settings.turnAcceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, 0, settings.rotationDeceleration * Time.fixedDeltaTime);
        }

        currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, -settings.maxTurnSpeed, settings.maxTurnSpeed);
        transform.Rotate(0, 0, currentRotationSpeed * Time.fixedDeltaTime);

        if (joystickInputValue.y == 0)
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, settings.forwardDeceleration * Time.fixedDeltaTime);
        }

        rigidbody.linearVelocity = Vector2.ClampMagnitude(currentVelocity, settings.maxForwardSpeed);
    }
}