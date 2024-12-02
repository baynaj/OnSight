using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuLoad : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject startMainMenu;
    public GameObject mainMenu;
    public GameObject options;
    //public GameObject about;
    //public GameObject exitConfirm;

    [Header("Main Menu Buttons")]
    public Button startMenuButton;
    public Button optionButton;
    //public Button aboutButton;
    public Button exitConfirmButton;
    public Button backButton;

    public List<Button> returnButtons;

    // Start is called before the first frame update
    void Start()
    {
        startMenuButton.onClick.AddListener(EnableInGameMenu);

        optionButton.onClick.AddListener(EnableOption);
        //aboutButton.onClick.AddListener(EnableAbout);
        exitConfirmButton.onClick.AddListener(EnableExitGame);
        backButton.onClick.AddListener(EnableGoBack);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableInGameMenu);
        }
    }

    private void EnableExitGame()
    {
        // Quit the game in the editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    public void HideAllInGameMenu()
    {
        startMainMenu.SetActive(true);
        mainMenu.SetActive(false);
        options.SetActive(false);
        //about.SetActive(false);
    }

    public void EnableInGameMenu()
    {
        startMainMenu.SetActive(false);
        mainMenu.SetActive(true);
        options.SetActive(false);
        //about.SetActive(false);
    }
    public void EnableOption()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
        //about.SetActive(false);
    }
    public void EnableAbout()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        //about.SetActive(true);
    }
    public void EnableGoBack()
    {
        HideAllInGameMenu();
    }
}
