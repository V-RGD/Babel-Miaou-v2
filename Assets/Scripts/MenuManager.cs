using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    
    public bool isLoading;
    public bool quitWarningActive;
    public bool gameIsPaused;
    public bool isInOptions;
    public PlayerControls playerControls;
    public InputAction quitMenu;

    public GameObject loadingSlider;
    public GameObject quitWarning;
    public GameObject loadingUI;
    public GameObject pauseMenu;
    public GameObject optionMenu;
    public DrawItemBox drawMenu;
    public GameObject deathPanel;
    
    private GameManager _gameManager;
    private ObjectsManager _objectsManager;
    private UIManager _uiManager;
    private CheatManager _cheatManager;
    private bool _canActiveDeathPanel = true;
    private bool _canPause = true;
    public bool canEscapeObjectMenu = true;
    private bool isInObjectMenu;
    private bool isInCommandLine;

    private void OnEnable()
    {
        quitMenu = playerControls.Menus.Escape;
        quitMenu.performed += EscapeButton;
        quitMenu.Enable();
    }

    private void OnDisable()
    {
        quitMenu.Disable();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        playerControls = new PlayerControls();
    }

    private void Start()
    {
        _gameManager = GameManager.instance;
        _objectsManager = ObjectsManager.instance;
        _uiManager = UIManager.instance;
        _cheatManager = CheatManager.instance;
    }

    void EscapeButton(InputAction.CallbackContext context)
    {
        if (isInCommandLine)
        {
            _cheatManager.CloseCommandLine();
            return;
        }
            
        if (isInOptions)
        {
            SettingsMenu();
            return;
        }

        bool slotsFilled = true;
        foreach (var slot in _objectsManager.itemObjectsInventory)
        {
            if (slot == 999)
            {
                slotsFilled = false;
                break;
            }
        }

        if (!_objectsManager.canReplaceItem || (_objectsManager.canReplaceItem && slotsFilled) )
        {
            if (isInObjectMenu)
            {
                //resets every position
                
                //then closes menu
                ObjectMenu();
                return;
            }
            PauseMenu();
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Quote))
        {
            //freezes game
            if (!isInCommandLine)
            {
                _cheatManager.OpenCommandLine();
            }
            else
            {
                _cheatManager.CloseCommandLine();
            }
        }

        if (isInCommandLine && Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            _cheatManager.CloseCommandLine();
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
            _objectsManager.itemObjectsInventory[3] = 999;
            //disables menu
            _objectsManager.objectMenu.SetActive(false);
            //resume time
            Time.timeScale = 1;
            //can pause
            _canPause = true;
            _objectsManager.canReplaceItem = false;
            _uiManager.UpdateHUDIcons();
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
            _objectsManager.canReplaceItem = true;
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
    
