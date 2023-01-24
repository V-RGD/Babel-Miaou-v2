using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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
    public GameObject mainMenuPanel;
    public GameObject tuto;
    public TMP_Text[] scoreTxt;
    public TMP_Text loadingAdvice;
    public string[] loadingAdvices;
    private AudioSource _audioSource;
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
        _audioSource = GetComponent<AudioSource>();
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
        _audioSource.Play();
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
        loadingAdvice.text = loadingAdvices[Random.Range(0, loadingAdvices.Length)];
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
    public void DisplayLeaderBoards()
    {
        if (leaderBoardMenu.activeInHierarchy)
        {
            leaderBoardMenu.SetActive(false);
            mainMenuPanel.SetActive(true);
            return;
        }
        
        leaderBoardMenu.SetActive(true);
        mainMenuPanel.SetActive(false);
        //updates scores
        scoreTxt[0].text = PlayerPrefs.GetInt("Score1").ToString();
        scoreTxt[1].text = PlayerPrefs.GetInt("Score2").ToString();
        scoreTxt[2].text = PlayerPrefs.GetInt("Score3").ToString();
        scoreTxt[3].text = PlayerPrefs.GetInt("Score4").ToString();
        scoreTxt[4].text = PlayerPrefs.GetInt("Score5").ToString();
    }
    public void DisplayTutorial()
    {
        if (tuto.activeInHierarchy)
        {
            tuto.SetActive(false);
            mainMenuPanel.SetActive(true);
            return;
        }
        tuto.SetActive(true);
        mainMenuPanel.SetActive(false);
    }
}