using UnityEngine;

/// <summary>
/// Set the rotation of an object
/// </summary>
public class RotateObject : MonoBehaviour
{
    [Tooltip("The value at which the speed is applied")]
    [Range(0, 1)] public float sensitivity = 1.0f;

    [Tooltip("The max speed of the rotation")]
    public float speed = 10.0f;

    [Tooltip("Whether to rotate in the opposite direction")]
    public bool rotateBackward = false;

    private bool isRotating = false;

    public void SetIsRotating(bool value)
    {
        if(value)
        {
            Begin();
        }
        else
        {
            End();
        }
    }

    public void Begin()
    {
        isRotating = true;
    }

    public void End()
    {
        isRotating = false;
    }

    public void ToggleRotate()
    {
        isRotating = !isRotating;
    }

    public void StartRotating(bool reverse)
    {
        isRotating = true;
        rotateBackward = reverse;
    }
    public void StopRotating()
    {
        isRotating = false;
    }

    public void SetSpeed(float value)
    {
        sensitivity = Mathf.Clamp(value, 0, 1);
        isRotating = (sensitivity * speed) != 0.0f;
    }

    private void FixedUpdate()
    {
        if (isRotating)
            if (rotateBackward)
                RotateBackward();
            else
                RotateForward();

    }

    public void RotateForward()
    {
        transform.Rotate(transform.up, (sensitivity * speed) * Time.deltaTime);
    }

    private void RotateBackward()
    {
        transform.Rotate(transform.up, (sensitivity * -speed) * Time.deltaTime);
    }
}
