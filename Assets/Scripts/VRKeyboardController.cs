using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VRKeyboardController : MonoBehaviour
{
    public GameObject vrKeyboard; // Reference to the VR keyboard GameObject
    public GameStartMenu gameStartMenu; // Reference to the GameStartMenu script
    private TMP_InputField activeInputField;

    // Start is called before the first frame update
    void Start()
    {
        vrKeyboard.SetActive(false);
    }

    public void ShowKeyboard(TMP_InputField inputField)
    {
        activeInputField = inputField;
        vrKeyboard.SetActive(true);
        //vrKeyboard.transform.position = inputField.transform.position + new Vector3(0, 0.5f, 0);
    }

    public void HideKeyboardAndGoBack()
    {
        vrKeyboard.SetActive(false);
        
        if (activeInputField != null)
        {
            activeInputField.text = ""; // Clear the input field
            activeInputField = null;
        }

        if (gameStartMenu != null)
        {
            gameStartMenu.ReactivateRecordNameButton();
        }
    }

    public void HideKeyboard()
    {
        vrKeyboard.SetActive(false);

        if (gameStartMenu != null)
        {
            gameStartMenu.ReactivateRecordNameButton();
        }
    }

    public void KeyPress(string key)
    {
        if (activeInputField != null)
        {
            activeInputField.text += key;
        }
    }

    public void Backspace()
    {
        if (activeInputField != null && activeInputField.text.Length > 0)
        {
            activeInputField.text = activeInputField.text.Substring(0, activeInputField.text.Length - 1);
        }
    }

}
