using UnityEngine;

/// <summary>
/// Change the color a meterial using a color, or Hue
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class SetKinematic : MonoBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetKinematicState(bool state)
    {
        rb.isKinematic = state;
    }

    public void ToggleKinematic()
    {
        SetKinematicState(!rb.isKinematic);
    }
}