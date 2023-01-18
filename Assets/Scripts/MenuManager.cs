using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    #region Variables
    public static MenuManager instance;
    
    public GameState gameState;
    public bool isLoading;
    private PlayerControls _playerControls;
    private InputAction _cancel;
    private InputAction _menu;
    private InputAction _confirm;
    private InputAction _upArrow;
    private InputAction _downArrow;
    private InputAction _commandLine;
    private GameManager _gameManager;
    private ObjectsManager _objectsManager;
    private UIManager _uiManager;
    private CheatManager _cheatManager;

    [Header("Warnings")]
    public GameObject quitWarning;
    public GameObject quitToMainMenuWarning;
    public GameObject discardItemWarning;
    public GameObject discardDrawWarning;
    [Header("Menus")]
    public GameObject loadingUI;
    public GameObject pauseMenu;
    public GameObject optionMenu;
    public DrawItemBox drawMenu;
    public GameObject deathPanel;

    [Header("Buttons")]
    public int buttonPos;
    public Button[] buttonsMainMenu;
    public Button[] buttonsPause;
    public Button[] buttonsOptions;
    public Button[] buttonsExitGame;
    public Button[] buttonsInventory;
    public Button[] buttonsDraw;
    public Button[] buttonsDiscardInventory;
    public Button[] buttonsDeath;
    public Button[] buttonsDiscardDraw;
    public Button[] buttonsExitToMainMenu;
    public Button[] buttonsLeaderBoard;

    public Animator pauseMenuAnimator;
    public Animator objectMenuAnimator;
    public Animator drawMenuAnimator;

    public GameObject drawMenuPanel;
    public GameObject objectMenu;
    
    #endregion
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
    private void OnEnable()
    {
        _menu = _playerControls.Menus.Menu;
        _menu.performed += MenuShortcut;
        _menu.Enable();
        
        _cancel = _playerControls.Menus.Cancel;
        _cancel.performed += EscapeButton;
        _cancel.Enable();
        
        _confirm = _playerControls.Menus.Confirm;
        _confirm.performed += ConfirmButton;
        _confirm.Enable();
        
        _upArrow = _playerControls.Menus.UpArrow;
        _upArrow.performed += UpButton;
        _upArrow.Enable();
        
        _downArrow = _playerControls.Menus.DownArrow;
        _downArrow.performed += DownButton;
        _downArrow.Enable();
        
        _commandLine = _playerControls.Menus.Console;
        _commandLine.performed += CommandLineShortcut;
        _commandLine.Enable();
        
    }
    private void OnDisable()
    {
        _menu.Disable();
        _confirm.Disable();
        _upArrow.Disable();
        _downArrow.Disable();
        _commandLine.Disable();
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        _playerControls = new PlayerControls();

        pauseMenuAnimator = pauseMenu.GetComponent<Animator>();
        drawMenuAnimator = drawMenu.GetComponent<Animator>();
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
    public void ObjectMenu()
    {
        if (gameState == GameState.Inventory)
        {
            //deletes 6th box object
            _objectsManager.itemObjectsInventory[3] = 999;
            //disables menu
            StartCoroutine(CloseMenu(objectMenu, objectMenuAnimator, GameState.Play));
            //can pause
            _objectsManager.canReplaceItem = false;
            _uiManager.UpdateHUDIcons();
            return;
        }
        
        if (gameState == GameState.Play)
        {
            //actives ui
            Debug.Log("tried to open object menu");
            StartCoroutine(OpenMenu(objectMenu, objectMenuAnimator, GameState.Inventory));
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
        if (!_objectsManager.canReplaceItem)
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
                // PauseMenu();
            }
        }
    }
    public void SwitchState(GameState newState)
    {
        gameState = newState;
    }
    public void PauseMenu()
    {
        if (gameState == GameState.Play)
        {
            StartCoroutine(OpenMenu(pauseMenu, pauseMenuAnimator, GameState.Pause));
        }

        if (gameState == GameState.Pause)
        {
            StartCoroutine(CloseMenu(pauseMenu, pauseMenuAnimator, GameState.Play));
        }
    }
    public void DrawMenu()
    {
        if (gameState == GameState.Play)
        {
            StartCoroutine(OpenMenu(drawMenu.gameObject, drawMenuAnimator, GameState.Draw));
        }

        if (gameState == GameState.Draw)
        {
            StartCoroutine(CloseMenu(pauseMenu, pauseMenuAnimator, GameState.Play));
        }
    }
    public void InventoryMenu()
    {
        if (gameState == GameState.Play)
        {
            StartCoroutine(OpenMenu(objectMenu, objectMenuAnimator, GameState.Inventory));
        }

        if (gameState == GameState.Inventory)
        {
            StartCoroutine(CloseMenu(objectMenu, objectMenuAnimator, GameState.Play));
        }
    }

    public bool canChangeMenu;
    public IEnumerator SwitchMenu(GameObject oldMenu, Animator oldAnimator, GameState newState, GameObject newMenu, Animator newAnimator)
    {
        //to regain movement
        if (newState != GameState.Play)
        {
            PlayerAttacks.instance.enabled = false;
            PlayerController.instance.enabled = false;
        }
        
        //bloque le changement de menu
        canChangeMenu = false;
        
        //ferme l'éventuel vieux menu
        oldAnimator.CrossFadeInFixedTime(Animator.StringToHash("Close"), 0);

        //attend un peu
        yield return new WaitForSeconds(0.5f);
        
        //ouvre le nouveau menu et ferme l'ancien
        newMenu.SetActive(true);
        oldMenu.SetActive(false);
        //attend un peu
        
        yield return new WaitForSeconds(0.5f);

        //change le state
        gameState = newState;
        
        //peut changer de menu
        canChangeMenu = true;

        //to regain movement
        if (gameState == GameState.Play)
        {
            PlayerAttacks.instance.enabled = true;
            PlayerController.instance.enabled = true;
        }
        else
        {
            PlayerAttacks.instance.enabled = false;
            PlayerController.instance.enabled = false;
        }
    }
    
    public IEnumerator OpenMenu(GameObject newMenu, Animator newAnimator, GameState newState)
    {
        //to avoid movement
        PlayerAttacks.instance.enabled = false;
        PlayerController.instance.enabled = false;
        
        //bloque le changement de menu
        canChangeMenu = false;
        
        //ouvre le nouveau menu
        newMenu.SetActive(true);
        newAnimator.CrossFadeInFixedTime(Animator.StringToHash("Open"), 0);
        //attend un peu
        
        yield return new WaitForSeconds(0.5f);

        //change le state
        gameState = newState;
        
        //peut changer de menu
        canChangeMenu = true;

        //to regain movement
        if (gameState == GameState.Play)
        {
            PlayerAttacks.instance.enabled = true;
            PlayerController.instance.enabled = true;
        }
        else
        {
            PlayerAttacks.instance.enabled = false;
            PlayerController.instance.enabled = false;
        }
    }
    public IEnumerator CloseMenu(GameObject oldMenu, Animator oldAnimator, GameState newState)
    {
        //bloque le changement de menu
        canChangeMenu = false;
        
        //ferme l'éventuel vieux menu
        oldAnimator.CrossFadeInFixedTime(Animator.StringToHash("Close"), 0);

        //attend un peu
        yield return new WaitForSeconds(0.5f);
        
        //ferme l'ancien
        oldMenu.SetActive(false);
        
        //change le state
        gameState = newState;
        
        //peut changer de menu
        canChangeMenu = true;

        //to regain movement
        if (gameState == GameState.Play)
        {
            PlayerAttacks.instance.enabled = true;
            PlayerController.instance.enabled = true;
        }
        else
        {
            PlayerAttacks.instance.enabled = false;
            PlayerController.instance.enabled = false;
        }
    }
    void CheckGameActive()
    {
        if (gameState is GameState.Death 
            or GameState.Console 
            or GameState.Draw 
            or GameState.Inventory
            or GameState.Option 
            or GameState.Pause 
            or GameState.DiscardDraw 
            or GameState.DiscardInventory 
            or GameState.QuitToMainMenuPrompt)
        {
            //disables playercontroller + enemies
            PlayerAttacks.instance.enabled = false;
            PlayerController.instance.enabled = false;
            Time.timeScale = 0;
        }
        else
        {
            //disables playercontroller + enemies
            PlayerAttacks.instance.enabled = true;
            PlayerController.instance.enabled = true;
            Time.timeScale = 1;
        }
    }
    public void UseSelectedButton(Button[] buttonList)
    {
        //uses currently selected button
        buttonList[buttonPos].onClick.Invoke();
        buttonPos = 0;
    }
    void ReplaceObjectInInventory()
    {
        if (_objectsManager.canReplaceItem)
        {
            _objectsManager.ReplaceItem(_objectsManager.currentBoxPos, _objectsManager.itemObjectsInventory[3]);
        }
    }

    #region Shortcuts
    private void ConfirmButton(InputAction.CallbackContext context)
    {
        switch (gameState)
        {
            case GameState.Pause : 
                UseSelectedButton(buttonsPause);
                break;
            case GameState.QuitToMainMenuPrompt : 
                UseSelectedButton(buttonsExitToMainMenu);
                break;
            case GameState.Option : 
                UseSelectedButton(buttonsOptions);
                break;
            case GameState.Draw :
                //select draw item
                drawMenu.AccessToItemMenu(drawMenu.items[buttonPos]);
                break;
            case GameState.Console : 
                //use console
                _cheatManager.CloseCommandLine();
                break;
            case GameState.MainMenu : 
                UseSelectedButton(buttonsMainMenu);
                break;
            case GameState.DiscardDraw : 
                UseSelectedButton(buttonsDiscardDraw);
                break;
            case GameState.LeaderBoard : 
                UseSelectedButton(buttonsLeaderBoard);
                break;
            case GameState.Inventory : 
                //replace object
                ReplaceObjectInInventory();
                break;
            case GameState.DiscardInventory : 
                UseSelectedButton(buttonsDiscardInventory);
                break;
            case GameState.Death : 
                UseSelectedButton(buttonsDeath);
                break;
            case GameState.ExitGamePrompt : 
                UseSelectedButton(buttonsExitGame);
                break;
        }
        CheckGameActive();
    }
    private void UpButton(InputAction.CallbackContext context)
    {
        switch (gameState)
        {
            case GameState.Console : 
                _cheatManager.InputLastCommand();
                break;
            case GameState.Inventory : 
                //moves inventory box
                buttonPos++;
                break;
            
            //moves cursor
            case GameState.Pause or
             GameState.QuitToMainMenuPrompt or
             GameState.Option or
             GameState.MainMenu or
             GameState.DiscardDraw or
             GameState.LeaderBoard or
             GameState.DiscardInventory or
             GameState.Death or
             GameState.ExitGamePrompt or GameState.Draw : 
                buttonPos++;
                break;
        }
    }
    private void DownButton(InputAction.CallbackContext context)
    {
        switch (gameState)
        {
            case GameState.Inventory : 
                //moves inventory box
                buttonPos--;
                break;
            
            //moves cursor
            case GameState.Pause or
                GameState.QuitToMainMenuPrompt or
                GameState.Option or
                GameState.MainMenu or
                GameState.DiscardDraw or
                GameState.LeaderBoard or
                GameState.DiscardInventory or
                GameState.Death or
                GameState.ExitGamePrompt or GameState.Draw : 
                buttonPos--;
                break;
        }
    }
    public void EscapeButton(InputAction.CallbackContext context)
    {
        switch (gameState)
        {
            // case GameState.Pause or GameState.Play: 
            //     //StartCoroutine(PauseMenu());
            //     break;
            // case GameState.QuitToMainMenuPrompt :
            //     //disables warning
            //     //to pause
            //     quitToMainMenuWarning.SetActive(false);
            //     SwitchState(GameState.Pause);
            //     break;
            // case GameState.Option : 
            //     optionMenu.SetActive(false);
            //     SwitchState(GameState.Pause);
            //     break;
            
            case GameState.Draw :
                //discard prompt
                break;
            case GameState.DiscardDraw :
                break;
            case GameState.Console : 
                //use console
                _cheatManager.CloseCommandLine();
                break;
            case GameState.MainMenu : 
                //show exit warning prompt
                break;
            case GameState.LeaderBoard : 
                break;
            case GameState.Inventory : 
                EscapeInventory();
                break;
            case GameState.DiscardInventory : 
                break;
            case GameState.ExitGamePrompt : 
                //to 
                break;
        }
        CheckGameActive();
    }
    private void CommandLineShortcut(InputAction.CallbackContext context)
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
        CheckGameActive();
    }
    private void MenuShortcut(InputAction.CallbackContext context)
    {
        switch (gameState)
        {
            case GameState.Play or GameState.Pause: 
                //activates pause
                PauseMenu();
                Debug.Log("found function");
                break;
            case GameState.QuitToMainMenuPrompt :
                //disables warning
                //to pause
                quitToMainMenuWarning.SetActive(false);
                SwitchState(GameState.Pause);
                break;
            case GameState.Option : 
                optionMenu.SetActive(false);
                SwitchState(GameState.Pause);
                break;
            case GameState.Inventory :
                break;
        }
        //CheckGameActive();
    }
    #endregion

}
    
