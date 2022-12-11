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

    private bool isPlayerInRange;
    private bool isActive;
    private float activeDist = 5;
    private bool canActive;
    
    private void Awake()
    {
        playerControls = new PlayerControls();
        canvas = GameObject.Find("UI Canvas");
        player = GameObject.Find("Player");
    }

    private void Start()
    {
        
        _uiManager = UIManager.instance;
        _menuManager = MenuManager.instance;
    }
    
    void ShopItem()
    {
        if (isPlayerInRange)
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
        }
    }

    #region InputSystemRequirements
    private void OnEnable()
    {
        collect = playerControls.Player.Collect;
        collect.Enable();
        collect.performed += Collect;
    }

    private void OnDisable()
    {
        collect.Disable();
    }
    #endregion
}
