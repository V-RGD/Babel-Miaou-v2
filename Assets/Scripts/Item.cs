using System;
using System.Collections.Generic;
using System.Linq;
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

    public bool isFromAShop;
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
        canvas = GameObject.Find("UI Canvas");
        player = GameObject.Find("Player");
    }

    private void Start()
    {
        
        gameManager = GameManager.instance;
        _uiManager = UIManager.instance;
        _menuManager = MenuManager.instance;
        _objectsManager = ObjectsManager.instance;
        costPrompt.GetComponent<TMP_Text>().text = itemCost.ToString();
        costPrompt.SetActive(false);
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
                int healAmount = 8;
                gameManager.health += healAmount;
                //caps health to the max amount
                if (gameManager.health > gameManager.maxHealth)
                {
                    gameManager.health = gameManager.maxHealth;
                }
                _uiManager.HealthBar(gameManager.health);
                break;
            case ItemType.MaxHealth :
                int maxHealthAmount = 4;
                gameManager.maxHealth += maxHealthAmount;
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
        else
        {
            costPrompt.SetActive(false);
        }
    }
    void Collect(InputAction.CallbackContext context)
    {
        if (isPlayerInRange && isFromAShop && _menuManager.gameState == MenuManager.GameState.Pause)
        {
            if (gameManager.money >= itemCost)
            {
                //does gameobject effect
                gameManager.money -= itemCost;
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
        _objectsManager.uiItemBoxes[3].SetActive(true);
        _menuManager.ObjectMenu();
        //puts it in the 6th box
        int newItem = objectID;
        _objectsManager.itemObjectsInventory[3] = newItem;
        //updates it's id
        _objectsManager.uiItemBoxes[3].transform.GetChild(3).GetComponent<Image>().sprite = _objectsManager.objectSprites[objectID];
        _objectsManager.UiItemBoxesUpdate();
        Destroy(gameObject);
    }
    
    void RandomObjectDraw()
    {
        _menuManager.drawMenu.gameObject.SetActive(true);
        List<int> doNotChooseTheSameObjectList = new List<int>();
        for (int i = 0; i < _objectsManager.itemList.Count; i++)
        {
            //add every possible item to the list
            doNotChooseTheSameObjectList.Add(_objectsManager.itemList[i]);
        }
        //actives a canvas to choose 2 objects from
        for (int i = 0; i < 2; i++)
        {
            //assigns box object with a random item
            int item = doNotChooseTheSameObjectList[Random.Range(0, doNotChooseTheSameObjectList.Count)];
            doNotChooseTheSameObjectList.Remove(item);
            //update : box name, icon, description
            string name = _objectsManager.itemDataScriptable.names[item];
            Sprite icon = _objectsManager.objectSprites[item];
            string desc = _objectsManager.itemDataScriptable.descriptions[item];

            _menuManager.drawMenu.items[i] = item;
            _menuManager.drawMenu.boxVisuals[i].description.text = desc;
            _menuManager.drawMenu.boxVisuals[i].name.text = name;
            _menuManager.drawMenu.boxVisuals[i].icon.sprite = icon;
        }
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
