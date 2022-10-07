using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    private PlayerControls playerControls;
    private UIManager _uiManager;
    private ObjectsManager _objectsManager;
    [HideInInspector]public InputAction collect;
    private GameManager gameManager;

    private GameObject player;
    private GameObject canvas;
    public GameObject costPrompt;

    [HideInInspector]public bool isFromAShop;
    private bool isPlayerInRange;
    private bool canBeTaken = true;
    private float grabDist = 5;
    
    //object info
    public int objectID;
    public string description;
    public string itemName;
    public int itemCost;
    public int rarity;

    private void Awake()
    {
        playerControls = new PlayerControls();
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
        costPrompt.GetComponent<TMP_Text>().text = itemCost + " noeuils";
        costPrompt.SetActive(false);
        canvas = GameObject.Find("UI Canvas");
    }


    private void Update()
    {
        ShopItem();
    }

    void ShopItem()
    {
        //check whether the item is from the shop or not
        if (isFromAShop)
        {
            //can collect just with a press
            itemCost = 0;

            if (isPlayerInRange)
            {
                costPrompt.SetActive(true);
            }
            else
            {
                costPrompt.SetActive(false);
            }
            
            if ((player.transform.position - transform.position).magnitude <= grabDist)
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
    }
    void Collect(InputAction.CallbackContext context)
    {
        if (isPlayerInRange && gameManager.money >= itemCost && isFromAShop)
        {
            //does gameobject effect
            gameManager.money -= itemCost;
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFromAShop && canBeTaken)
        {
            canBeTaken = false;
            _uiManager.ActiveObjectMenu();
            //instantiates a new ui item in the canvas
            GameObject newItem = Instantiate(_objectsManager.uiItemPrefab, canvas.transform);
            _objectsManager.itemObjectsInventory[5] = newItem;
            //puts it in the 6th box
            _objectsManager.itemObjectsInventory[5].GetComponent<RectTransform>().transform.position = _objectsManager.uiItemBoxes[5].transform.position;
            //updates it's id
            _objectsManager.itemObjectsInventory[5].GetComponent<ItemDragDrop>().objectID = objectID;
            _objectsManager.itemObjectsInventory[5].GetComponent<Image>().sprite = _objectsManager.objectSprites[objectID];
            _objectsManager.itemObjectsInventory[5].GetComponent<ItemDragDrop>().boxAssociated = 5;
            Destroy(gameObject);
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
