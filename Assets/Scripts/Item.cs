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
    public TMP_Text costText;
    public GameObject pancarte;

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
        player = GameObject.Find("Player");
    }

    private void Start()
    {
        pancarte = transform.GetChild(0).transform.GetChild(0).gameObject;
        costText = pancarte.transform.GetComponentInChildren<TMP_Text>();
        
        gameManager = GameManager.instance;
        _uiManager = UIManager.instance;
        _menuManager = MenuManager.instance;
        _objectsManager = ObjectsManager.instance;
        //sets item cost on screen
        costText.text = itemCost.ToString();
        //then sets item visuals
        switch (itemType)
        {
            case ItemType.Heal :
                spriteRenderer1.sprite = _uiManager.healthSprite;
                break;
            case ItemType.MaxHealth :
                spriteRenderer1.sprite = _uiManager.maxHealthSprite;
                break;
            case ItemType.Item :
                SetItemDrops(1);
                break;
            case ItemType.RandomItem : 
                SetItemDrops(2);
                break;
        }
        pancarte.SetActive(false);
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
                gameManager.healFx.Play();
                break;
            case ItemType.MaxHealth :
                int maxHealthAmount = 4;
                gameManager.maxHealth += maxHealthAmount;
                gameManager.health += maxHealthAmount;
                _uiManager.HealthBar(gameManager.health);
                //enlarges healthbar
                gameManager.maxHpFx.Play();
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
                pancarte.SetActive(true);
            }
            else
            {
                pancarte.SetActive(false);
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
            pancarte.SetActive(false);
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
        objectID = item1;
        int newItem = objectID;
        _objectsManager.itemObjectsInventory[3] = newItem;
        //updates it's id
        //_objectsManager.uiItemBoxes[3].transform.GetChild(3).GetComponent<Image>().sprite = _objectsManager.objectSprites[objectID];
        int id = _objectsManager.itemObjectsInventory[3];
        //update : box name, icon, description, rarity color
        string name = _objectsManager.itemDataScriptable.names[id];
        Sprite icon = _objectsManager.objectSprites[id];
        string desc = _objectsManager.itemDataScriptable.descriptions[id];
        _objectsManager.uiItemBoxes[3].transform.GetChild(1).GetComponent<TMP_Text>().text = name;
        _objectsManager.uiItemBoxes[3].transform.GetChild(2).GetComponent<TMP_Text>().text = desc;
        _objectsManager.uiItemBoxes[3].transform.GetChild(3).GetComponent<Image>().enabled = true;
        _objectsManager.uiItemBoxes[3].transform.GetChild(3).GetComponent<Image>().sprite = icon;
        _objectsManager.UiItemBoxesUpdate();
        Destroy(gameObject);
    }
    
    void RandomObjectDraw()
    {
        _menuManager.drawMenu.gameObject.SetActive(true);
        PlayerController.instance.enabled = false;
        PlayerAttacks.instance.enabled = false;
        // List<int> doNotChooseTheSameObjectList = new List<int>();
        // for (int i = 0; i < _objectsManager.itemList.Count; i++)
        // {
        //     //add every possible item to the list
        //     doNotChooseTheSameObjectList.Add(_objectsManager.itemList[i]);
        // }
        //actives a canvas to choose 2 objects from
        for (int i = 0; i < 2; i++)
        {
            int item = 0;
            switch (i)
            {
                case 0 : item = item1; break;
                case 1 : item = item2; break;
            }
            // //assigns box object with a random item
            // int item = doNotChooseTheSameObjectList[Random.Range(0, doNotChooseTheSameObjectList.Count)];
            // doNotChooseTheSameObjectList.Remove(item);
            //update : box name, icon, description
            string name = _objectsManager.itemDataScriptable.names[item];
            Sprite icon = _objectsManager.objectSprites[item];
            string desc = _objectsManager.itemDataScriptable.descriptions[item];

            _menuManager.drawMenu.items[i] = item;
            _menuManager.drawMenu.boxVisuals[i].description.text = desc;
            _menuManager.drawMenu.boxVisuals[i].name.text = name;
            _menuManager.drawMenu.boxVisuals[i].icon.sprite = icon;
            _menuManager.drawMenu.isMenuActive = true;
        }
    }
    public int item1;
    public int item2;
    public SpriteRenderer spriteRenderer1;
    public SpriteRenderer spriteRenderer2;

    private void SetItemDrops(int numberOfDrops)
    {
        if (numberOfDrops == 1)
        {
            //sets item
            List<int> doNotChooseTheSameObjectList = new List<int>();
            for (int i = 0; i < _objectsManager.itemList.Count; i++)
            {
                //add every possible item to the list
                doNotChooseTheSameObjectList.Add(_objectsManager.itemList[i]);
            }
            //actives a canvas to choose 2 objects from
            //assigns box object with a random item
            item1 = doNotChooseTheSameObjectList[Random.Range(0, doNotChooseTheSameObjectList.Count)];
            doNotChooseTheSameObjectList.Remove(item1);
            //updates icon
            spriteRenderer1.sprite = _objectsManager.objectSprites[item1];
        }

        else if (numberOfDrops == 2)
        {
            //sets both loots
            //sets item
            List<int> doNotChooseTheSameObjectList = new List<int>();
            for (int i = 0; i < _objectsManager.itemList.Count; i++)
            {
                //add every possible item to the list
                doNotChooseTheSameObjectList.Add(_objectsManager.itemList[i]);
            }
            //actives a canvas to choose 2 objects from
            //assigns box object with a random item
            item1 = doNotChooseTheSameObjectList[Random.Range(0, doNotChooseTheSameObjectList.Count)];
            doNotChooseTheSameObjectList.Remove(item1);
            item2 = doNotChooseTheSameObjectList[Random.Range(0, doNotChooseTheSameObjectList.Count)];
            doNotChooseTheSameObjectList.Remove(item2);
            //updates icon
            spriteRenderer1.sprite = _objectsManager.objectSprites[item1];
            spriteRenderer2.sprite = _objectsManager.objectSprites[item2];
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
