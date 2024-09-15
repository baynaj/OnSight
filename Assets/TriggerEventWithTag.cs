using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Calls functionality when a collision occurs
/// </summary>
public class TriggerEventWithTag : MonoBehaviour
{
    // variable to store the tag whitelist
    public string[] tags;

    [Serializable] public class CollisionEvent : UnityEvent<Collision> { }

    [Serializable] public class TriggerEvent : UnityEvent<Collider> { }

    // When the object enters a collision
    public TriggerEvent OnEnter = new TriggerEvent();

    // When the object exits a collision
    public TriggerEvent OnExit = new TriggerEvent();


    private void OnTriggerEnter(Collider other)
    {
        if (tags.Length == 0)
        {
            OnEnter.Invoke(other);
            return;
        }

        foreach (var tag in tags)
        {
            if (other.gameObject.CompareTag(tag))
            {
                OnEnter.Invoke(other);
                return;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (tags.Length == 0)
        {
            OnEnter.Invoke(other);
            return;
        }

        foreach (var tag in tags)
        {
            if (other.gameObject.CompareTag(tag))
            {
                OnExit.Invoke(other);
                return;
            }
        }
    }

    private void OnValidate()
    {
        if (TryGetComponent(out Collider collider))
            collider.isTrigger = true;
    }
}
