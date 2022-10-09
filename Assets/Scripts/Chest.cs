using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Chest : MonoBehaviour
{

    private bool isPlayerInRange;
    private float openDist = 5;
    private PlayerControls _playerControls;
    private InputAction _collect;
    private GameObject _player;
    public GameObject messagePrompt;
    public ObjectsManager objectManager;
    public bool isOpen;

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _player = GameObject.Find("Player");
        objectManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
        _collect = _playerControls.Player.Collect;
    }

    void Collect(InputAction.CallbackContext context)
    {
        if (isPlayerInRange)
        {
            //spawns a random item between items, spells, or loot
            GameObject item = Instantiate(objectManager.chestPool[Random.Range(0, objectManager.chestPool.Count)], transform.position,
                quaternion.identity);
            item.SetActive(true);
            isOpen = true;
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if ((_player.transform.position - transform.position).magnitude <= openDist)
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
        _collect.Enable();
        _collect.performed += Collect;
    }

    private void OnDisable()
    {
        _collect.Disable();
    }
    #endregion
}
