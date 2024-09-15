using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Calls functionality when a collision occurs
/// </summary>
public class OnCollisionWithTag : MonoBehaviour
{
    // variable to store the tag whitelist
    public string[] tags;

    [Serializable] public class CollisionEvent : UnityEvent<Collision> { }

    // When the object enters a collision
    public CollisionEvent OnEnter = new CollisionEvent();

    // When the object exits a collision
    public CollisionEvent OnExit = new CollisionEvent();

    
    

    private void OnCollisionEnter(Collision collision)
    {
        if (tags.Length == 0)
        {
            OnEnter.Invoke(collision);
            return;
        }

        foreach (var tag in tags)
        {
            if (collision.gameObject.CompareTag(tag))
            {
                OnEnter.Invoke(collision);
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (tags.Length == 0)
        {
            OnExit.Invoke(collision);
            return;
        }

        foreach (var tag in tags)
        {
            if (collision.gameObject.CompareTag(tag))
            {
                OnExit.Invoke(collision);
                return;
            }
        }
    }

    private void OnValidate()
    {
        if (TryGetComponent(out Collider collider))
            collider.isTrigger = false;
    }
}
