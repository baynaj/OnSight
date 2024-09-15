using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public class InGameMenuController : MonoBehaviour
{
    public GameObject menu; 

    private bool isMenuActive = false;

    public XRController controller;

    public bool useSimulator = true;

    //void Update()
    //{
    //    // Check if the Y button is pressed
    //    if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
    //    {
    //        ToggleMenu();
    //    }
    //}

    //void ToggleMenu()
    //{
    //    isMenuActive = !isMenuActive; 
    //    menu.SetActive(isMenuActive);
    //}

    void Update()
    {
        if (useSimulator)
        {
            // XR Device Simulator input check using new Input System
            if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame) // Change to your desired key for simulation
            {
                ToggleMenu();
            }
        }
        else
        {
            // Actual VR input check
            if (controller != null && controller.inputDevice.isValid)
            {
                if (controller.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
                {
                    ToggleMenu();
                }
            }
            else
            {
                Debug.LogWarning("Controller or input device is not set up correctly.");
            }
        }
    }

    void ToggleMenu()
    {
        if (menu != null)
        {
            isMenuActive = !isMenuActive; // Toggle the menu state
            menu.SetActive(isMenuActive); // Set menu active or inactive
        }
        else
        {
            Debug.LogWarning("Menu GameObject is not assigned.");
        }
    }
}
