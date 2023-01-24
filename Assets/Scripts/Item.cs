using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    private PlayerControls playerControls;
    [HideInInspector]public InputAction collect;
    public ShopManager shopManager;

    private GameObject player;
    public TMP_Text costText;
    public GameObject pancarte;

    public bool isFromAShop;
    private bool isPlayerInRange;
    public bool canBeTaken = false;
    private float grabDist = 5;
    
    //object info
    public int objectID;
    public string description;
    public string itemName;
    public int itemCost;
    public int rarity;
    public bool isDuplicate;
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

    private IEnumerator Start()
    {
        if (isFromAShop)
        {
            canBeTaken = true;
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
            canBeTaken = true;
        }
        
        pancarte = transform.GetChild(0).transform.GetChild(0).gameObject;
        costText = pancarte.transform.GetComponentInChildren<TMP_Text>();
        
        //sets item cost on screen
        //then sets item visuals
        switch (itemType)
        {
            case ItemType.Heal :
                spriteRenderer1.sprite = UIManager.instance.healthSprite;
                break;
            case ItemType.MaxHealth :
                spriteRenderer1.sprite = UIManager.instance.maxHealthSprite;
                break;
            case ItemType.Item :
                SetItemDrops(1);
                break;
            case ItemType.RandomItem : 
                SetItemDrops(2);
                break;
        }

        if (!isDuplicate)
        {
            switch (itemType)
            {
                case ItemType.Heal :
                    itemCost = 10;
                    break;
                case ItemType.MaxHealth :
                    itemCost = 15;
                    break;
                case ItemType.Item :
                    itemCost = 0;
                    break;
                case ItemType.RandomItem : 
                    itemCost = 20;
                    break;
            }
        }
        costText.text = itemCost.ToString();
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
                GameManager.instance.health += healAmount;
                //caps health to the max amount
                if (GameManager.instance.health > GameManager.instance.maxHealth)
                {
                    GameManager.instance.health = GameManager.instance.maxHealth;
                }
                UIManager.instance.HealthBar(GameManager.instance.health);
                GameManager.instance.healFx.Play();
                if (isFromAShop)
                {
                    GameObject healObject = Instantiate(ObjectsManager.instance.maxHealthItem, transform.position, Quaternion.Euler(0, -45, 0));
                    healObject.GetComponent<Item>().isFromAShop = true;
                    healObject.GetComponent<Item>().isDuplicate = true;
                    healObject.GetComponent<Item>().itemCost = Mathf.CeilToInt(itemCost * 1.3f);
                    healObject.transform.parent = transform;
                    healObject.SetActive(true);
                }
                
                break;
            case ItemType.MaxHealth :
                int maxHealthAmount = 4;
                GameManager.instance.maxHealth += maxHealthAmount;
                GameManager.instance.health += maxHealthAmount;
                UIManager.instance.HealthBar(GameManager.instance.health);
                //enlarges healthbar
                GameManager.instance.maxHpFx.Play();
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
            if (isPlayerInRange && itemCost == 0)
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
        if (isPlayerInRange && canBeTaken)
        {
            if (GameManager.instance.money >= itemCost)
            {
                //can buy health if already max health
                if (itemType == ItemType.Heal && GameManager.instance.health >= GameManager.instance.maxHealth)
                {
                    return;
                }
                //does gameobject effect
                GameManager.instance.money -= itemCost;
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
        ObjectsManager.instance.uiItemBoxes[3].SetActive(true);
        MenuManager.instance.ObjectMenu();
        //puts it in the 6th box
        objectID = item1;
        int newItem = objectID;
        ObjectsManager.instance.itemObjectsInventory[3] = newItem;
        //updates it's id
        //ObjectsManager.instance.uiItemBoxes[3].transform.GetChild(3).GetComponent<Image>().sprite = ObjectsManager.instance.objectSprites[objectID];
        int id = ObjectsManager.instance.itemObjectsInventory[3];
        //update : box name, icon, description, rarity color
        string name = ObjectsManager.instance.itemDataScriptable.names[id];
        Sprite icon = ObjectsManager.instance.objectSprites[id];
        string desc = ObjectsManager.instance.itemDataScriptable.descriptions[id];
        ObjectsManager.instance.uiItemBoxes[3].transform.GetChild(1).GetComponent<TMP_Text>().text = name;
        ObjectsManager.instance.uiItemBoxes[3].transform.GetChild(2).GetComponent<TMP_Text>().text = desc;
        ObjectsManager.instance.uiItemBoxes[3].transform.GetChild(3).GetComponent<Image>().enabled = true;
        ObjectsManager.instance.uiItemBoxes[3].transform.GetChild(3).GetComponent<Image>().sprite = icon;
        ObjectsManager.instance.UiItemBoxesUpdate();
        Destroy(gameObject);
    }
    
    void RandomObjectDraw()
    {
        MenuManager.instance.drawMenu.gameObject.SetActive(true);
        PlayerController.instance.enabled = false;
        PlayerAttacks.instance.enabled = false;
        // List<int> doNotChooseTheSameObjectList = new List<int>();
        // for (int i = 0; i < ObjectsManager.instance.itemList.Count; i++)
        // {
        //     //add every possible item to the list
        //     doNotChooseTheSameObjectList.Add(ObjectsManager.instance.itemList[i]);
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
            string name = ObjectsManager.instance.itemDataScriptable.names[item];
            Sprite icon = ObjectsManager.instance.objectSprites[item];
            string desc = ObjectsManager.instance.itemDataScriptable.descriptions[item];

            MenuManager.instance.drawMenu.items[i] = item;
            MenuManager.instance.drawMenu.boxVisuals[i].description.text = desc;
            MenuManager.instance.drawMenu.boxVisuals[i].name.text = name;
            MenuManager.instance.drawMenu.boxVisuals[i].icon.sprite = icon;
            MenuManager.instance.drawMenu.isMenuActive = true;
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
            for (int i = 0; i < ObjectsManager.instance.itemList.Count; i++)
            {
                //add every possible item to the list
                doNotChooseTheSameObjectList.Add(ObjectsManager.instance.itemList[i]);
            }
            //actives a canvas to choose 2 objects from
            //assigns box object with a random item
            item1 = doNotChooseTheSameObjectList[Random.Range(0, doNotChooseTheSameObjectList.Count)];
            doNotChooseTheSameObjectList.Remove(item1);
            //updates icon
            spriteRenderer1.sprite = ObjectsManager.instance.objectSprites[item1];
        }

        else if (numberOfDrops == 2)
        {
            //sets both loots
            //sets item
            List<int> doNotChooseTheSameObjectList = new List<int>();
            for (int i = 0; i < ObjectsManager.instance.itemList.Count; i++)
            {
                //add every possible item to the list
                doNotChooseTheSameObjectList.Add(ObjectsManager.instance.itemList[i]);
            }
            //actives a canvas to choose 2 objects from
            //assigns box object with a random item
            item1 = doNotChooseTheSameObjectList[Random.Range(0, doNotChooseTheSameObjectList.Count)];
            doNotChooseTheSameObjectList.Remove(item1);
            item2 = doNotChooseTheSameObjectList[Random.Range(0, doNotChooseTheSameObjectList.Count)];
            doNotChooseTheSameObjectList.Remove(item2);
            //updates icon
            spriteRenderer1.sprite = ObjectsManager.instance.objectSprites[item1];
            spriteRenderer2.sprite = ObjectsManager.instance.objectSprites[item2];
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
