using TheProphecy.Player;
using UnityEngine;
using TheProphecy.Interfaces;

public class GamepadMovement : IMovement
{
    public void HandleMovement(Transform transform, VirtualJoystick joystickInput, Rigidbody2D rigidbody, MovementController.MovementSettings settings, float rotationOffset, ref Vector2 currentVelocity, ref float currentRotationSpeed)
    {
        Vector2 joystickInputValue = joystickInput.GetJoystickInput();

        // Debug: Log the joystick input values
      //  Debug.Log($"Joystick Input: {joystickInputValue}, Magnitude: {joystickInputValue.magnitude}");

        Vector2 targetVelocity = Vector2.zero;

        // Check if there's significant joystick input
        if (joystickInputValue.magnitude > 0.1f)
        {
            // Calculate the target angle based on joystick input
            float targetAngle = Mathf.Atan2(joystickInputValue.y, joystickInputValue.x) * Mathf.Rad2Deg + rotationOffset;

            // Smoothly rotate towards the target angle
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, settings.turnAcceleration * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Calculate the forward direction based on current rotation
            Vector2 forwardDirection = transform.up;

            // Set target velocity in the forward direction
            targetVelocity = forwardDirection * settings.maxForwardSpeed;
        }

        // Smoothly interpolate current velocity towards target velocity
        if (targetVelocity != Vector2.zero)
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, settings.forwardAcceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Gradually reduce velocity when no input is detected
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, settings.forwardDeceleration * Time.fixedDeltaTime);
        }

        // Clamp the speed to maximum speed
        currentVelocity = Vector2.ClampMagnitude(currentVelocity, settings.maxForwardSpeed);

        // Apply the calculated linear velocity
        rigidbody.linearVelocity = currentVelocity; // Use linearVelocity to avoid warnings
    }
}