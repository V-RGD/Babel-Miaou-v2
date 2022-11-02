using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public GameObject deathPanel;
    
    private GameManager _gameManager;
    private ObjectsManager _objectsManager;

    private bool _canActiveDeathPanel = true;
    private bool _canPause = true;
    public bool canEscapeObjectMenu = true;
    private bool isInObjectMenu;


    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
    }

    private void Update()
    {
        //escape shortcut
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isInOptions)
            {
                SettingsMenu();
                return;
            }

            if (isInObjectMenu)
            {
                ObjectMenu();
                return;
            }
            PauseMenu();
        }

        if (quitWarningActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            MainMenu();
        }
        if (_gameManager.isDead && _canActiveDeathPanel)
        {
            _canActiveDeathPanel = false;
            DeathPanel();
        }
    }


    #region fonctionne
    public void StartGame()
    {
        StartCoroutine(LoadingScreen());
    }
    public void MainMenu()
    {
        //shortcuts for use
        if (quitWarningActive)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            quitWarningActive = true;
            quitWarning.SetActive(true);
        }
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
    public void PauseMenu()
    {
        if (!gameIsPaused)
        {
            gameIsPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            gameIsPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }
    #endregion
    
    public void ObjectMenu()
    {
        if (isInObjectMenu && canEscapeObjectMenu)
        {
            isInObjectMenu = false;
            //deletes 6th box object
            Destroy(_objectsManager.itemObjectsInventory[5]);
            _objectsManager.itemObjectsInventory[5] = null;
            //disables menu
            _objectsManager.objectMenu.SetActive(false);
            //resume time
            Time.timeScale = 1;
            //can pause
            _canPause = true;
            return;
        }
        
        if (!isInObjectMenu)
        {
            isInObjectMenu = true;
            //actives ui
            _objectsManager.objectMenu.SetActive(true);
            //stop time
            Time.timeScale = 0;
            //can't pause
            _canPause = false;
            return;
        }
    }
    
    #region WarningPrompt
    
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

    public void DeathPanel()
    {
        deathPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void DiscardWarningPrompt()
    {
        quitWarningActive = false;
        quitWarning.SetActive(false);
    }
}
    