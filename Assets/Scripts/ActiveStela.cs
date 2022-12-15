using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActiveStela : MonoBehaviour
{
    private PlayerControls playerControls;
    private MenuManager _menuManager;
    [HideInInspector]public InputAction collect;
    public Room room;
    private UIManager _uiManager;

    private GameObject player;
    private GameObject canvas;
    public GameObject activePrompt;

    public bool isPlayerInRange;
    public bool isActive;
    public float activeDist = 5;
    public bool canActive = true;
    
    private void Awake()
    {
        playerControls = new PlayerControls();
        canvas = GameObject.Find("UI Canvas");
        player = GameObject.Find("Player");
    }

    private void Start()
    {
        canActive = true;
        _uiManager = UIManager.instance;
        _menuManager = MenuManager.instance;
    }
    
    void ShowPrompt()
    {
        if (isPlayerInRange & canActive)
        {
            activePrompt.SetActive(true);
        }
        else
        {
            activePrompt.SetActive(false);
        }
            
        if ((player.transform.position - transform.position).magnitude <= activeDist)
        {
            //show message prompt
            isPlayerInRange = true;
        }
        else
        {
            //disable message prompt
            isPlayerInRange = false;
        }
    }
    void Collect(InputAction.CallbackContext context)
    {
        if (isPlayerInRange && !_menuManager.gameIsPaused && canActive)
        {
            canActive = false;
            //actives stela
            room.isStelaActive = true;
            //room.StartCoroutine(room.ActivateAllEnemies());
        }
    }

    private void Update()
    {
        ShowPrompt();
    }

    #region InputSystemRequirements
    private void OnEnable()
    {
        collect = playerControls.Player.Interact;
        collect.Enable();
        collect.performed += Collect;
    }

    private void OnDisable()
    {
        collect.Disable();
    }
    #endregion
}
