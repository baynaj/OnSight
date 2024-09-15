using UnityEngine;

/// <summary>
/// Destroys object after a few seconds or when triggered
/// </summary>
public class DestroyObject : MonoBehaviour
{
    [Tooltip("Time before destroying in seconds")]
    public bool triggerOnStart = true;
    public float lifeTime = 5.0f;

    private void Start()
    {
        if(triggerOnStart)
            Destroy(gameObject, lifeTime);
    }

    public void TriggerDestroy()
    {
        Destroy(gameObject);
    }
}
