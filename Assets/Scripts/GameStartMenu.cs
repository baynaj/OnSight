using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject options;
    public GameObject about;
    public GameObject recordName;
    public GameObject enterName;

    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button optionButton;
    public Button aboutButton;
    //public Button recordNameButton;
    public Button enterNameButton;
    public Button quitButton;
    public Button submitButton;

    public TMP_InputField nameInputField;

    public VRKeyboardController vrKeyboardController;

    public List<Button> returnButtons;

    // Start is called before the first frame update
    void Start()
    {
        nameInputField.onSelect.AddListener(delegate { EnableVRKeyboard(); });
        EnableMainMenu();
        //Hook events
        //startButton.onClick.AddListener(StartGame); // not needed for now since we have multiple scenes to choose from
        startButton.onClick.AddListener(EnableTypeName);
        optionButton.onClick.AddListener(EnableOption);
        aboutButton.onClick.AddListener(EnableAbout);
        quitButton.onClick.AddListener(QuitGame);
        //recordNameButton.onClick.AddListener(EnableRecordName);
        //typeNameButton.onClick.AddListener(EnableTypeName);
        submitButton.onClick.AddListener(OnSubmit);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }

    public void QuitGame()
    {
        // Quit the game in the editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    public void StartGame()
    {
        HideAll();
        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
        recordName.SetActive(false);
        enterName.SetActive(false);
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
        about.SetActive(false);
        recordName.SetActive(false);
        enterName.SetActive(false);
    }
    public void EnableOption()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
        about.SetActive(false);
        recordName.SetActive(false);
        enterName.SetActive(false);
    }
    public void EnableAbout()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(true);
        recordName.SetActive(false);
        enterName.SetActive(false);
    }
    public void EnableRecordName()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
        //recordName.SetActive(true);
        enterName.SetActive(false);
    }
    public void EnableTypeName()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
        enterName.SetActive(true);
        recordName.SetActive(true);
    }

    public void EnableVRKeyboard()
    {
        vrKeyboardController.ShowKeyboard(nameInputField);

        Debug.Log($"VRKeyboard active status: {vrKeyboardController.vrKeyboard.activeInHierarchy}");
        Debug.Log($"recordName active status before: {recordName.activeSelf}");

        if (vrKeyboardController.vrKeyboard.activeInHierarchy)
        {
            recordName.SetActive(false);
        }
    }

    public void ReactivateRecordNameButton()
    {
        if (recordName != null)
        {
            recordName.SetActive(true); // Reactivate the recordName button
            Debug.Log("recordName button reactivated");
        }
    }

    public void OnSubmit()
    {
        if (nameInputField == null)
        {
            Debug.LogError("nameInputField is not assigned!");
            return;
        }

        string userName = nameInputField.text;
        if (!string.IsNullOrEmpty(userName))
        {
            PlayerPrefs.SetString("UserName", userName);
            //Debug.Log("username saved: " + userName);
            
            Debug.Log("username saved: " + PlayerPrefs.GetString("UserName"));
            // Start the game
            StartGame();
        }
        else
        {
            Debug.Log("Name field is empty!");
        }
    }
}
