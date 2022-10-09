using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{
    public bool isLoading;
    public bool quitWarningActive;
    public bool gameIsPaused;
    public bool isInOptions;

    public GameObject loadingSlider;
    public GameObject quitWarning;
    public GameObject loadingUI;
    public GameObject pauseMenu;
    public GameObject optionMenu;

    private GameObject _player;

    public GameObject sceneID;
    public GameObject minimapUI;

    private void Awake()
    {
        _player = GameObject.Find("Player");
    }

    private void Update()
    {
        if (sceneID == null)
        {
            ExitGameWarning();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu();
        }
        
        if (quitWarningActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            MainMenu();
        }
    }

    public void StartGame()
    {
        StartCoroutine(LoadingScreen());
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        
            if (!quitWarningActive)
            {
                ShowUpExitWarning();
            }

            //shortcuts for use
            if (quitWarningActive)
            {
                MainMenu();
            }
    }

    public void RestartLevel()
    {
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
            pauseMenu.SetActive(true);
        }
        else
        {
            isInOptions = false;
            pauseMenu.SetActive(false);
        }
    }

    public void PauseMenu()
    {
        if (!gameIsPaused)
        {
            gameIsPaused = true;
            Debug.Log("pause ");
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            gameIsPaused = false;
            Debug.Log("unpause ");
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }

    #region WarningPrompt
    
    public void ShowUpExitWarning()
    {
        if (!quitWarningActive)
        {
            quitWarningActive = true;
            quitWarning.SetActive(true);
        }
        else
        {
            DiscardWarningPrompt();
        }
    }
    private void ExitGameWarning()
    {
        
    }

    public void DiscardWarningPrompt()
    {
        quitWarningActive = false;
        quitWarning.SetActive(false);
        //cache la souris
    }
    #endregion
    IEnumerator LoadingScreen()
    {
        loadingUI.SetActive(true);
        //does nothing the first second
        yield return new WaitForSeconds(1);
        loadingSlider.GetComponent<Animator>().SetTrigger("LoadingStart");
        //loads for 3 seconds
        yield return new WaitForSeconds(3);
        //change scene
        SceneManager.LoadScene("MainScene");
    }
}
    
