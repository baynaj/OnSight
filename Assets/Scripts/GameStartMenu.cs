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
    //public GameObject recordName;
    public GameObject typeName;

    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button optionButton;
    public Button aboutButton;
    //public Button recordNameButton;
    public Button typeNameButton;
    public Button quitButton;
    public Button submitButton;

    public TMP_InputField nameInputField;

    public List<Button> returnButtons;

    // Start is called before the first frame update
    void Start()
    {
        EnableMainMenu();

        //Hook events
        //startButton.onClick.AddListener(StartGame); // not needed for now since we have multiple scenes to choose from
        optionButton.onClick.AddListener(EnableOption);
        aboutButton.onClick.AddListener(EnableAbout);
        quitButton.onClick.AddListener(QuitGame);
        //recordNameButton.onClick.AddListener(EnableRecordName);
        typeNameButton.onClick.AddListener(EnableTypeName);
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
        //recordName.SetActive(false);
        typeName.SetActive(false);
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
        about.SetActive(false);
        //recordName.SetActive(false);
        typeName.SetActive(false);
    }
    public void EnableOption()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
        about.SetActive(false);
        //recordName.SetActive(false);
        typeName.SetActive(false);
    }
    public void EnableAbout()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(true);
        //recordName.SetActive(false);
        typeName.SetActive(false);
    }
    public void EnableRecordName()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
        //recordName.SetActive(true);
        typeName.SetActive(false);
    }
    public void EnableTypeName()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
        //recordName.SetActive(false);
        typeName.SetActive(true);
        
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
            //StartGame();
        }
        else
        {
            Debug.Log("Name field is empty!");
        }
    }
}
