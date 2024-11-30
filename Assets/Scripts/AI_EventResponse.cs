using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_EventResponse : MonoBehaviour
{

    [SerializeField] private AI_Manager promptedAI = null;
    [SerializeField] private bool isOneShot = true;
    
    [Tooltip("The tags that will activate the trigger.")]
    [SerializeField] private string[] checkedTags = { "Player" };

    [Tooltip("The message to send to the AI when the event is triggered.\n" +
        "Example: 'The user has walked through the door'")]
    [TextArea(3, 50)] public string eventMessage = "";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(eventMessage != "", "Event message is empty! Event will be deactivated!");
        Debug.Assert(promptedAI == null, "No AI Manager selected! Event will be deactivated!");
        if (eventMessage == "" || promptedAI == null) gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendEventMessage(string message)
    {
        promptedAI.GenerateAiResponse(message);
    }

    private void OnTriggerEnter(Collider other)
    {
        //this is to allow a set of tags to be checked rather than a single tag in the editor
        foreach (string tag in checkedTags)
        {
            if (other.CompareTag(tag))
            {
                SendEventMessage($"EVENT: {eventMessage}");
                if (isOneShot) gameObject.SetActive(false);
                break;
            }
        }
    }
}
