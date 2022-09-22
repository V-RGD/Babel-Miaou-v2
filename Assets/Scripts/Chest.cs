using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Chest : MonoBehaviour
{

    private bool isPlayerInRange;
    private float openDist = 5;
    public PlayerControls playerControls;
    public InputAction collect;
    public GameObject player;
    public GameObject messagePrompt;
    public GameManager gameManager;
    public bool isOpen;

    private void Awake()
    {
        playerControls = new PlayerControls();
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Collect(InputAction.CallbackContext context)
    {
        if (isPlayerInRange)
        {
            //spawns item
            Instantiate(gameManager.items[Random.Range(0, gameManager.items.Length)], transform.position,
                quaternion.identity);
            isOpen = true;
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if ((player.transform.position - transform.position).magnitude <= openDist)
        {
            //show message prompt
            isPlayerInRange = true;
            messagePrompt.SetActive(true);
        }
        else
        {
            //disable message prompt
            isPlayerInRange = false;
            messagePrompt.SetActive(false);
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
