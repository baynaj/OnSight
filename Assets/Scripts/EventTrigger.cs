using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class EventTrigger : MonoBehaviour
{
    public List<string> triggerTags = new();
    public bool oneShot = false;
    public UnityEvent customEvent = null;
    

    // Start is called before the first frame update

    //trigger an arbitrary function on an arbirary script on an arbitrary object
    private void OnTriggerEnter(Collider other)
    {
        foreach(string tag in triggerTags)
        {
            if (customEvent != null && other.CompareTag(tag))
            {
                customEvent.Invoke();
                break;
            }
        }

        if (oneShot)
        {
            gameObject.SetActive(false);
        }
    }



}
