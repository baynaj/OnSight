using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null; // Singleton instance
    public bool isGameRunning = false; // Track if the game has started

    void Awake()
    {
        // If an instance already exists and it's not this one, destroy this instance
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            Debug.Log("Duplicate GameManager destroyed");
        }
        else
        {
            // Make this the singleton instance and don't destroy it on scene load
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("GameManager instance set and DontDestroyOnLoad called");
        }
    }
}
