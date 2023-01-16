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
    public PlayerControls playerControls;
    public InputAction quit;
    public InputAction confirm;
    public InputAction upArrow;
    public InputAction downArrow;
    public InputAction commandLine;

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
    public enum GameState
    {
        Play,
        Pause,
        Option,
        QuitToMainMenuPrompt,

        LeaderBoard,
        Loading,
        Console,
        
        MainMenu,
        ExitGamePrompt,
        Tutorial,
        
        Inventory,
        DiscardInventory,
        
        Draw,
        DiscardDraw,
        
        Death
    }

    public GameState gameState;
    private void OnEnable()
    {
        quit = playerControls.Menus.Escape;
        quit.performed += EscapeButton;
        quit.Enable();
        
        confirm = playerControls.Menus.Confirm;
        confirm.performed += ConfirmButton;
        confirm.Enable();
        
        upArrow = playerControls.Menus.UpArrow;
        upArrow.performed += UpButton;
        upArrow.Enable();
        
        downArrow = playerControls.Menus.DownArrow;
        downArrow.performed += DownButton;
        downArrow.Enable();
    }
    private void OnDisable()
    {
        quit.Disable();
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

        if (isLoading)
        {
            loadingUI.SetActive(true);
            StartCoroutine(StartLevel());
        }
    }
    
    private void ConfirmButton(InputAction.CallbackContext context)
    {
        switch (gameState)
        {
            case GameState.Loading : break;
            case GameState.Play : break;
            case GameState.Pause : break;
            case GameState.QuitToMainMenuPrompt : break;
            case GameState.Option : break;
            case GameState.Draw : break;
            case GameState.Console : break;
            case GameState.MainMenu : break;
            case GameState.DiscardDraw : break;
            case GameState.Tutorial : break;
            case GameState.LeaderBoard : break;
            case GameState.Inventory : break;
            case GameState.DiscardInventory : break;
            case GameState.Death : break;
            case GameState.ExitGamePrompt : break;
        }
    }
    private void UpButton(InputAction.CallbackContext context)
    {
        switch (gameState)
        {
            case GameState.Pause : break;
            case GameState.QuitToMainMenuPrompt : break;
            case GameState.Option : break;
            case GameState.Draw : break;
            case GameState.Console : break;
            case GameState.MainMenu : break;
            case GameState.DiscardDraw : break;
            case GameState.LeaderBoard : break;
            case GameState.Inventory : break;
            case GameState.DiscardInventory : break;
            case GameState.Death : break;
            case GameState.ExitGamePrompt : break;
        }
    }
    private void DownButton(InputAction.CallbackContext context)
    {
        switch (gameState)
        {
            case GameState.Pause : break;
            case GameState.QuitToMainMenuPrompt : break;
            case GameState.Option : break;
            case GameState.Draw : break;
            case GameState.Console : break;
            case GameState.MainMenu : break;
            case GameState.DiscardDraw : break;
            case GameState.LeaderBoard : break;
            case GameState.Inventory : break;
            case GameState.DiscardInventory : break;
            case GameState.Death : break;
            case GameState.ExitGamePrompt : break;
        }
    }
    public void EscapeButton(InputAction.CallbackContext context)
    {
        switch (gameState)
        {
            case GameState.Play : break;
            case GameState.Pause : break;
            case GameState.QuitToMainMenuPrompt : break;
            case GameState.Option : break;
            case GameState.Draw : break;
            case GameState.Console : break;
            case GameState.MainMenu : break;
            case GameState.DiscardDraw : break;
            case GameState.Tutorial : break;
            case GameState.LeaderBoard : break;
            case GameState.Inventory : break;
            case GameState.DiscardInventory : break;
            case GameState.Death : break;
            case GameState.ExitGamePrompt : break;
        }
    }
    private void CommandLineShortcut()
    {
        switch (gameState)
        { 
            case GameState.Play : 
                _cheatManager.OpenCommandLine();
                SwitchState(GameState.Console);
                break;
            case GameState.Console : 
                _cheatManager.CloseCommandLine();
                SwitchState(GameState.Play);
                break;
        }
    }

    private void MenuShortCut()
    {
        switch (gameState)
        {
            case GameState.Play : break;
            case GameState.Pause : break;
            case GameState.QuitToMainMenuPrompt : break;
            case GameState.Option : break;
        }
    }
    private void Update()
    {
        if (_gameManager.isDead && gameState == GameState.Play)
        {
            SwitchState(GameState.Death);
            DeathPanel();
        }
    }
    public void StartGame()
    {
        StartCoroutine(LoadingScreen());
    }
    public void MainMenu()
    {
        //shortcuts for use
        if (gameState == GameState.QuitToMainMenuPrompt)
        {
            SwitchState(GameState.MainMenu);
            SceneManager.LoadScene("MainMenu");
        }

        if (gameState == GameState.Pause)
        {
            quitWarning.SetActive(true);
            SwitchState(GameState.QuitToMainMenuPrompt);
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
        if (gameState == GameState.Pause)
        {
            optionMenu.SetActive(true);
            SwitchState(GameState.Option);
        }

        if (gameState == GameState.Option)
        {
            optionMenu.SetActive(false);
            SwitchState(GameState.Pause);
        }
    }
    public void PauseMenu()
    {
        if (gameState == GameState.Play)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            SwitchState(GameState.Pause);
        }

        if (gameState == GameState.Pause)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            SwitchState(GameState.Play);
        }
    }
    public void ObjectMenu()
    {
        if (gameState == GameState.DiscardInventory)
        {
            //deletes 6th box object
            _objectsManager.itemObjectsInventory[3] = 999;
            //disables menu
            _objectsManager.objectMenu.SetActive(false);
            //resume time
            Time.timeScale = 1;
            //can pause
            _objectsManager.canReplaceItem = false;
            _uiManager.UpdateHUDIcons();
            SwitchState(GameState.Play);
            return;
        }
        
        if (gameState == GameState.Play)
        {
            SwitchState(GameState.Inventory);
            //actives ui
            _objectsManager.objectMenu.SetActive(true);
            //stop time
            Time.timeScale = 0;
            //can't pause
            _objectsManager.canReplaceItem = true;
        }
    }
    IEnumerator LoadingScreen()
    {
        SwitchState(GameState.Loading);
        PlayerController.instance.enabled = false;
        PlayerAttacks.instance.enabled = false;

        loadingUI.SetActive(true);
        //does nothing the first second
        yield return new WaitForSeconds(3);
        //change scene
        SceneManager.LoadScene("MainScene");
    }
    private void DeathPanel()
    {
        deathPanel.SetActive(true);
        
        Time.timeScale = 0;
    }
    public void DiscardWarningPrompt()
    {
        quitWarning.SetActive(false);
    }
    public IEnumerator StartLevel()
    {
        SwitchState(GameState.Loading);
        //waits for the level to start
        yield return new WaitUntil(()=> DunGen.instance.finishedGeneration);
        yield return new WaitForSeconds(2);
        //disables screen, enables character
        PlayerController.instance.enabled = true;
        PlayerAttacks.instance.enabled = true;
        loadingUI.SetActive(false);
        SwitchState(GameState.Play);
    }
    private void EscapeInventory()
    {
        bool slotsFilled = true;
        foreach (var slot in _objectsManager.itemObjectsInventory)
        {
            if (slot == 999)
            {
                slotsFilled = false;
                break;
            }
        }
        if ((_objectsManager.canReplaceItem && slotsFilled) )
        {
            if (gameState == GameState.Inventory)
            {
                //resets every position
                //then closes menu
                ObjectMenu();
                return;
            }
            PauseMenu();
        }
    }
    public void SwitchState(GameState newState)
    {
        gameState = newState;
    }
}
    
