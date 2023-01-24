using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public GameObject nextLevelPanel;
    public TMP_Text nextLevelTxt;
    public Animator winScreen;
    public Animator leaderBoardScreen;

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
    public bool canChangeMenu;

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

    private PlayerController _playerController;
    private PlayerAttacks _playerAttacks;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

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
        _playerController = PlayerController.instance;
        _playerAttacks = PlayerAttacks.instance;

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
            StartCoroutine(DeathPanel());
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(MenuManager.instance.EndGame());
        }
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
            Debug.Log("tried to escape object menu");
            //deletes 6th box object
            _objectsManager.itemObjectsInventory[3] = 999;
            //disables menu
            //can pause
            _objectsManager.canReplaceItem = false;
            _uiManager.UpdateHUDIcons();
            StartCoroutine(CloseMenu(objectMenu, objectMenuAnimator, GameState.Play));
            return;
        }
        
        if (gameState == GameState.Play)
        {
            //actives ui
            StartCoroutine(OpenMenu(objectMenu, objectMenuAnimator, GameState.Inventory));
            //can't pause
            _objectsManager.canReplaceItem = true;
        }
    }
    public void DiscardWarningPrompt()
    {
        quitWarning.SetActive(false);
    }
    // private void EscapeInventory()
    // {
    //     ObjectMenu();
    //     if (!_objectsManager.canReplaceItem)
    //     {
    //         bool slotsFilled = true;
    //         foreach (var slot in _objectsManager.itemObjectsInventory)
    //         {
    //             if (slot == 999)
    //             {
    //                 slotsFilled = false;
    //                 break;
    //             }
    //         }
    //         if ((_objectsManager.canReplaceItem && slotsFilled) )
    //         {
    //             if (gameState == GameState.Inventory)
    //             {
    //                 //resets every position
    //                 //then closes menu
    //                 return;
    //             }
    //             // PauseMenu();
    //         }
    //     }
    // }
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
    public IEnumerator SwitchMenu(GameObject oldMenu, Animator oldAnimator, GameState newState, GameObject newMenu, Animator newAnimator)
    {
        //to regain movement
        if (newState != GameState.Play)
        {
            _playerAttacks.enabled = false;
            _playerController.enabled = false;
            Time.timeScale = 0;
        }
        
        //bloque le changement de menu
        canChangeMenu = false;
        
        //ferme l'éventuel vieux menu
        oldAnimator.CrossFadeInFixedTime(Animator.StringToHash("Close"), 0);

        //attend un peu
        yield return new WaitForSecondsRealtime(0.5f);
        
        //ouvre le nouveau menu et ferme l'ancien
        newMenu.SetActive(true);
        oldMenu.SetActive(false);
        //attend un peu
        
        yield return new WaitForSecondsRealtime(0.5f);

        //change le state
        gameState = newState;
        
        //peut changer de menu
        canChangeMenu = true;

        //to regain movement
        if (gameState == GameState.Play)
        {
            _playerAttacks.enabled = true;
            _playerController.enabled = true;
            Time.timeScale = 1;
        }
        else
        {
            _playerAttacks.enabled = false;
            _playerController.enabled = false;
            Time.timeScale = 0;
        }
    }
    public IEnumerator OpenMenu(GameObject newMenu, Animator newAnimator, GameState newState)
    {
        //to avoid movement
        _playerAttacks.enabled = false;
        _playerController.enabled = false;
        Time.timeScale = 0;
        
        //bloque le changement de menu
        canChangeMenu = false;
        
        //ouvre le nouveau menu
        newMenu.SetActive(true);
        newAnimator.CrossFadeInFixedTime(Animator.StringToHash("Open"), 0);
        //attend un peu
        
        yield return new WaitForSecondsRealtime(0.5f);

        //change le state
        gameState = newState;
        
        //peut changer de menu
        canChangeMenu = true;

        //to regain movement
        if (gameState == GameState.Play)
        {
            _playerAttacks.enabled = true;
            _playerController.enabled = true;
            Time.timeScale = 1;
        }
        else
        {
            _playerAttacks.enabled = false;
            _playerController.enabled = false;
            Time.timeScale = 0;
        }
    }
    public IEnumerator CloseMenu(GameObject oldMenu, Animator oldAnimator, GameState newState)
    {
        //bloque le changement de menu
        canChangeMenu = false;
        
        //ferme l'éventuel vieux menu
        oldAnimator.CrossFadeInFixedTime(Animator.StringToHash("Close"), 0);

        //attend un peu
        yield return new WaitForSecondsRealtime(0.5f);
        
        //ferme l'ancien
        oldMenu.SetActive(false);
        
        //change le state
        gameState = newState;
        
        //peut changer de menu
        canChangeMenu = true;

        //to regain movement
        if (gameState == GameState.Play)
        {
            _playerAttacks.enabled = true;
            _playerController.enabled = true;
            Time.timeScale = 1;
        }
        else
        {
            _playerAttacks.enabled = false;
            _playerController.enabled = false;
        }
    }
    // void CheckGameActive()
    // {
    //     if (gameState is GameState.Death 
    //         or GameState.Console 
    //         or GameState.Draw 
    //         or GameState.Inventory
    //         or GameState.Option 
    //         or GameState.Pause 
    //         or GameState.DiscardDraw 
    //         or GameState.DiscardInventory 
    //         or GameState.QuitToMainMenuPrompt)
    //     {
    //         //disables playercontroller + enemies
    //         _playerAttacks.enabled = false;
    //         _playerController.enabled = false;
    //         Time.timeScale = 0;
    //     }
    //     else
    //     {
    //         //disables playercontroller + enemies
    //         _playerAttacks.enabled = true;
    //         _playerController.enabled = true;
    //         Time.timeScale = 1;
    //     }
    // }
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
        ObjectMenu();
    }
    IEnumerator LoadingScreen()
    {
        SwitchState(GameState.Loading);
        _playerController.enabled = false;
        _playerAttacks.enabled = false;
        Time.timeScale = 0;

        loadingUI.SetActive(true);
        //does nothing the first second
        yield return new WaitForSecondsRealtime(3);
        //change scene
        SceneManager.LoadScene("MainScene");
    }
    public GameObject[] deathPanelButtons;
    public Animator deathPanelAnimator;
    public Animator deathTitleAnimator;
    private IEnumerator DeathPanel()
    {
        _playerController.enabled = false;
        _playerAttacks.enabled = false;
        Time.timeScale = 0;
        deathPanel.SetActive(true);
        foreach (var button in deathPanelButtons)
        {
            button.SetActive(false);
        }
        deathPanelAnimator.CrossFadeInFixedTime(Animator.StringToHash("Open"), 0);
        deathTitleAnimator.CrossFadeInFixedTime(Animator.StringToHash("DeathTitleTraduction"), 0);
        yield return new WaitForSecondsRealtime(1.3f);
        foreach (var button in deathPanelButtons)
        {
            button.SetActive(true);
        }
    }
    public IEnumerator StartLevel()
    {
        _playerController.enabled = false;
        _playerAttacks.enabled = false;
        SwitchState(GameState.Loading);
        //waits for the level to start
        yield return new WaitUntil(()=> DunGen.instance.finishedGeneration);
        loadingUI.SetActive(false);
        //changes next level text
        switch (LevelManager.instance.currentLevel)
        {
            case 0 :
                nextLevelTxt.text = "Floor I";
                break;
            case 1 : 
                nextLevelTxt.text = "Floor II";
                break;
            case 2 : 
                nextLevelTxt.text = "Floor III";
                break;
            case 3 : 
                nextLevelTxt.text = "Big Meow's Lair";
                break;
        }
        //loading screen
        nextLevelPanel.SetActive(true);
        nextLevelPanel.GetComponent<Animator>().CrossFade(Animator.StringToHash("Load"), 0, 0);
        yield return new WaitForSeconds(6.5f);

        nextLevelPanel.SetActive(false);
        
        //disables screen, enables character
        GameObject player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().enabled = true;
        player.GetComponent<PlayerAttacks>().enabled = true;
        Time.timeScale = 1;
        loadingUI.SetActive(false);
        SwitchState(GameState.Play);
    }
    public void StartGame()
    {
        StartCoroutine(LoadingScreen());
    }
    public void RestartLevel()
    {
        deathPanel.SetActive(false);
        
        GameManager.instance.maxHealth = GameManager.instance.initialMaxHealth;
        GameManager.instance.health = GameManager.instance.initialMaxHealth;
        GameManager.instance.money = 0;
        GameScore.instance.tempScore = 0;

        for (int i = 0; i < ObjectsManager.instance.itemObjectsInventory.Count; i++)
        {
            ObjectsManager.instance.itemObjectsInventory[i] = 999;
        }
        
        UIManager.instance.HealthBar(GameManager.instance.health);
        loadingUI.SetActive(true);
        StartCoroutine(StartLevel());
        SceneManager.LoadScene("MainScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator EndGame()
    {
        Debug.Log("killed boss");
        _playerAttacks.enabled = false;
        _playerController.enabled = false;
        Time.timeScale = 0;
        MenuManager.instance.winScreen.gameObject.SetActive(true);
        MenuManager.instance.winScreen.CrossFade(Animator.StringToHash("Win"), 0);
        yield return new WaitForSecondsRealtime(4);
        MenuManager.instance.winScreen.gameObject.SetActive(false);
        StartCoroutine(MenuManager.instance.OpenMenu(MenuManager.instance.leaderBoardScreen.gameObject,
            MenuManager.instance.leaderBoardScreen, MenuManager.GameState.LeaderBoard));
        StartCoroutine(GameScore.instance.ShowLeaderBoards());
        //successes check
    }
    #region Shortcuts
    private void ConfirmButton(InputAction.CallbackContext context)
    {
        switch (gameState)
        {
            case GameState.Pause : 
                //UseSelectedButton(buttonsPause);
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
        //CheckGameActive();
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
            case GameState.Console : 
                //use console
                _cheatManager.CloseCommandLine();
                break;
            case GameState.Inventory : 
                // ObjectMenu();
                break;
            case GameState.DiscardInventory : 
                break;
            case GameState.ExitGamePrompt : 
                //to 
                break;
        }
        //CheckGameActive();
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
        //CheckGameActive();
    }
    private void MenuShortcut(InputAction.CallbackContext context)
    {
        switch (gameState)
        {
            case GameState.Play or GameState.Pause: 
                //activates pause
                PauseMenu();
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
        if (_menu == null)
        {
            return;
        }
        
        _menu.Disable();
        _confirm.Disable();
        _upArrow.Disable();
        _downArrow.Disable();
        _commandLine.Disable();
    }
}
    
