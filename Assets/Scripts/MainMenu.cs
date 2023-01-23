using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public bool quitWarningActive;
    public bool isInOptions;
    public PlayerControls playerControls;
    public InputAction quitMenu;

    public GameObject quitWarning;
    public GameObject loadingUI;
    public GameObject optionMenu;
    public GameObject leaderBoardMenu;
    private void OnEnable()
    {
        quitMenu = playerControls.Menus.Menu;
        quitMenu.performed += EscapeButton;
        quitMenu.Enable();
    }
    private void OnDisable()
    {
        quitMenu.Disable();
    }
    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void EscapeButton(InputAction.CallbackContext context)
    {
        if (isInOptions)
        {
            SettingsMenu();
        }

        if (leaderBoardMenu.activeInHierarchy)
        {
            leaderBoardMenu.SetActive(false);
        }
    }
    public void StartGame()
    {
        StartCoroutine(LoadingScreen());
    }
    public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void SettingsMenu()
    {
        if (!isInOptions)
        {
            isInOptions = true;
            optionMenu.SetActive(true);
        }
        else
        {
            isInOptions = false;
            optionMenu.SetActive(false);
        }
    }
    IEnumerator LoadingScreen()
    {
        loadingUI.SetActive(true);
        //does nothing the first second
        yield return new WaitForSeconds(3);
        //change scene
        SceneManager.LoadScene("MainScene");
    }
    public void DiscardWarningPrompt()
    {
        quitWarningActive = false;
        quitWarning.SetActive(false);
    }

    public void ShowLeaderBoards()
    {
        if (leaderBoardMenu.activeInHierarchy)
        {
            leaderBoardMenu.SetActive(false);
        }
        else
        {
            leaderBoardMenu.SetActive(true);
        }
    }
}