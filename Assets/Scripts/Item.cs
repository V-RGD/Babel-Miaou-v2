using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    private PlayerControls playerControls;
    private MenuManager _menuManager;
    private ObjectsManager _objectsManager;
    [HideInInspector]public InputAction collect;
    private GameManager gameManager;
    public ShopManager shopManager;
    private UIManager _uiManager;

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
    public ItemType itemType;
    public enum ItemType
    {
        Heal,
        MaxHealth,
        Item,
        RandomItem
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        _menuManager = GameObject.Find("UIManager").GetComponent<MenuManager>();
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
        costPrompt.GetComponent<TMP_Text>().text = itemCost.ToString();
        costPrompt.SetActive(false);
        canvas = GameObject.Find("UI Canvas");
    }


    private void Update()
    {
        ShopItem();
    }

    void ItemEffect()
    {
        switch (itemType)
        {
            case ItemType.Heal : 
                int healAmount = 4;
                gameManager.health += healAmount;
                _uiManager.HealthBar(gameManager.health);
                break;
            case ItemType.MaxHealth :
                int maxHealthAmount = 2;
                gameManager.maxHealth += maxHealthAmount;
                Debug.Log("maxHealth increased from "+ (gameManager.maxHealth - maxHealthAmount) + "to " + gameManager.maxHealth);
                gameManager.health += maxHealthAmount;
                _uiManager.HealthBar(gameManager.health);
                break;
            case ItemType.Item :
                AccessToItemMenu();
                break;
            case ItemType.RandomItem : 
                RandomObjectDraw();
                break;
        }
        Destroy(gameObject);
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
        if (isPlayerInRange && isFromAShop)
        {
            if (gameManager.money >= itemCost)
            {
                //does gameobject effect
                gameManager.money -= itemCost;
                ItemEffect();
            }
            else if (gameManager.health - itemCost/2 >= 1 && _objectsManager.strangePact)
            {
                //reduces cost by all money available
                float newCost = itemCost - gameManager.money;
                gameManager.money = 0;
                //new cost 
                gameManager.health -= Mathf.CeilToInt(newCost/2);
                ItemEffect();
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFromAShop && canBeTaken)
        {
            canBeTaken = false;
            ItemEffect();
        }
    }
    
    public void AccessToItemMenu()
    {
        _menuManager.ObjectMenu();
        //puts it in the 6th box
        int newItem = objectID;
        _objectsManager.itemObjectsInventory[5] = newItem;        //updates it's id
        _objectsManager.uiItemBoxes[5].GetComponent<Image>().sprite = _objectsManager.objectSprites[objectID];
        _objectsManager.UiItemBoxesUpdate();
        Destroy(gameObject);
    }
    
    void RandomObjectDraw()
    {
        _menuManager.drawMenu.gameObject.SetActive(true);
        //actives a canvas to choose 3 objects from
        for (int i = 0; i < 3; i++)
        {
            //assigns box object with a random item
            int item = shopManager.itemsToChooseFrom[Random.Range(0, shopManager.itemsToChooseFrom.Count)];
            //update : box name, icon, description, rarity color
            string name = _objectsManager.itemDataScriptable.names[item];
            Sprite icon = _objectsManager.objectSprites[item];
            string desc = _objectsManager.itemDataScriptable.descriptions[item];
            int rarity = _objectsManager.itemDataScriptable.rarity[item];
            Color color = Color.grey;
            
            switch (rarity)
            {
                case 1 : color = Color.green; break;
                case 2 : color = Color.blue; break;
                case 3 : color = Color.magenta; break;
                case 4 : color = Color.yellow; break;
            }
            _menuManager.drawMenu.items[i] = item;
            _menuManager.drawMenu.boxVisuals[i].description.text = desc;
            _menuManager.drawMenu.boxVisuals[i].name.text = name;
            _menuManager.drawMenu.boxVisuals[i].rarity.color = color;
            _menuManager.drawMenu.boxVisuals[i].icon.sprite = icon;
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
