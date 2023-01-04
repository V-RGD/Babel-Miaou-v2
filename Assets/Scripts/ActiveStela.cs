using UnityEngine;
using UnityEngine.InputSystem;

public class ActiveStela : MonoBehaviour
{
    private PlayerControls _playerControls;
    [HideInInspector]public InputAction collect;
    public Room room;

    private GameObject _player;
    public GameObject activePrompt;

    public bool isPlayerInRange;
    public bool isActive;
    public float activeDist = 5;
    public bool canActive = true;
    
    private void Awake()
    {
        _playerControls = new PlayerControls();
        _player = GameObject.Find("Player");
    }

    private void Start()
    {
        canActive = true;
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
            
        if ((_player.transform.position - transform.position).magnitude <= activeDist)
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
        if (isPlayerInRange && !MenuManager.instance.gameIsPaused && canActive)
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
        collect = _playerControls.Player.Interact;
        collect.Enable();
        collect.performed += Collect;
    }

    private void OnDisable()
    {
        collect.Disable();
    }
    #endregion
}
