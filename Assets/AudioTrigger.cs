using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Box"))
        {
            // Play the audio
            foreach(AudioSource audio in GetComponents<AudioSource>())
            {
                audio.Play();
            }
        }
    }
}
