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
    public GameObject about;

    [Header("Main Menu Buttons")]
    public Button startMenuButton;
    public Button optionButton;
    public Button aboutButton;
    public Button exitButton;

    public List<Button> returnButtons;

    // Start is called before the first frame update
    void Start()
    {
        startMenuButton.onClick.AddListener(EnableMainMenu);

        optionButton.onClick.AddListener(EnableOption);
        aboutButton.onClick.AddListener(EnableAbout);
        exitButton.onClick.AddListener(EnableExit);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }

    public void HideAllMainMenu()
    {
        startMainMenu.SetActive(true);
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
    }

    public void EnableMainMenu()
    {
        startMainMenu.SetActive(false);
        mainMenu.SetActive(true);
        options.SetActive(false);
        about.SetActive(false);
    }
    public void EnableOption()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
        about.SetActive(false);
    }
    public void EnableAbout()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(true);
    }
    public void EnableExit()
    {
        HideAllMainMenu();
    }
}
