using TheProphecy.Player;
using TheProphecy.Interfaces;
using UnityEngine;

public class KeyboardMovement : IMovement
{
    public void HandleMovement(Transform transform, VirtualJoystick joystickInput, Rigidbody2D rigidbody, MovementController.MovementSettings settings, float rotationOffset, ref Vector2 currentVelocity, ref float currentRotationSpeed)
    {
        Vector2 joystickInputValue = joystickInput.GetJoystickInput();

        if (joystickInputValue.y > 0)
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, transform.up * settings.maxForwardSpeed, settings.forwardAcceleration * Time.fixedDeltaTime);
        }
        else if (joystickInputValue.y < 0)
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, settings.forwardAcceleration * Time.fixedDeltaTime);
            if (currentVelocity.magnitude < 0.1f)
            {
                currentVelocity = Vector2.zero;
            }
        }

        if (joystickInputValue.x != 0)
        {
            float targetAngle = joystickInputValue.x < 0 ? -settings.maxTurnSpeed : settings.maxTurnSpeed;
            currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, -targetAngle, settings.turnAcceleration * Time.fixedDeltaTime);
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

        rigidbody.linearVelocity = currentVelocity;
    }
}