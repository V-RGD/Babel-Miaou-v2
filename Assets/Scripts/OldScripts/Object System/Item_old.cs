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

public class Item_old : MonoBehaviour
{
    private PlayerControls playerControls;
    [HideInInspector]public InputAction collect;
    public ShopManager_old shopManager;

    private GameObject player;
    public TMP_Text costText;
    public GameObject pancarte;

    public bool isFromAShop;
    private bool isPlayerInRange;
    public bool canBeTaken = false;
    private float grabDist = 10;
    private AudioSource source;
    
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
        source = GetComponent<AudioSource>();
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
                int healAmount = 50;
                GameManager_old.instance.health += healAmount;
                //caps health to the max amount
                if (GameManager_old.instance.health > GameManager_old.instance.maxHealth)
                {
                    GameManager_old.instance.health = GameManager_old.instance.maxHealth;
                }
                //source.PlayOneShot(GameSounds.instance.playerHeal[0]);
                UIManager.instance.HealthBar(GameManager_old.instance.health);
                GameManager_old.instance.healFx.Play();
                if (isFromAShop)
                {
                    GameObject healObject = Instantiate(ObjectsManager_old.instance.maxHealthItem, transform.position, Quaternion.Euler(0, -45, 0));
                    healObject.GetComponent<Item_old>().isFromAShop = true;
                    healObject.GetComponent<Item_old>().isDuplicate = true;
                    healObject.GetComponent<Item_old>().itemCost = Mathf.CeilToInt(itemCost * 1.3f);
                    healObject.transform.parent = transform;
                    healObject.SetActive(true);
                }
                
                break;
            case ItemType.MaxHealth :
                int maxHealthAmount = 4;
                GameManager_old.instance.maxHealth += maxHealthAmount;
                GameManager_old.instance.health += maxHealthAmount;
                UIManager.instance.HealthBar(GameManager_old.instance.health);
                //enlarges healthbar
                GameManager_old.instance.maxHpFx.Play();
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
            if (GameManager_old.instance.money >= itemCost)
            {
                //can buy health if already max health
                if (itemType == ItemType.Heal && GameManager_old.instance.health >= GameManager_old.instance.maxHealth)
                {
                    return;
                }
                //does gameobject effect
                GameManager_old.instance.money -= itemCost;
                if (isFromAShop)
                {
                    source.PlayOneShot(GameSounds.instance.itemPurchased[0]);
                }
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
        ObjectsManager_old.instance.uiItemBoxes[3].SetActive(true);
        MenuManager_old.instance.ObjectMenu();
        //puts it in the 6th box
        objectID = item1;
        int newItem = objectID;
        ObjectsManager_old.instance.itemObjectsInventory[3] = newItem;
        //updates it's id
        //ObjectsManager.instance.uiItemBoxes[3].transform.GetChild(3).GetComponent<Image>().sprite = ObjectsManager.instance.objectSprites[objectID];
        int id = ObjectsManager_old.instance.itemObjectsInventory[3];
        //update : box name, icon, description, rarity color
        string name = ObjectsManager_old.instance.itemDataScriptable.names[id];
        Sprite icon = ObjectsManager_old.instance.objectSprites[id];
        string desc = ObjectsManager_old.instance.itemDataScriptable.descriptions[id];
        ObjectsManager_old.instance.uiItemBoxes[3].transform.GetChild(1).GetComponent<TMP_Text>().text = name;
        ObjectsManager_old.instance.uiItemBoxes[3].transform.GetChild(2).GetComponent<TMP_Text>().text = desc;
        ObjectsManager_old.instance.uiItemBoxes[3].transform.GetChild(3).GetComponent<Image>().enabled = true;
        ObjectsManager_old.instance.uiItemBoxes[3].transform.GetChild(3).GetComponent<Image>().sprite = icon;
        ObjectsManager_old.instance.UiItemBoxesUpdate();
        Destroy(gameObject);
    }
    
    void RandomObjectDraw()
    {
        MenuManager_old.instance.drawMenu.gameObject.SetActive(true);
        PlayerController__old.instance.enabled = false;
        PlayerAttacks_old.instance.enabled = false;
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
            string name = ObjectsManager_old.instance.itemDataScriptable.names[item];
            Sprite icon = ObjectsManager_old.instance.objectSprites[item];
            string desc = ObjectsManager_old.instance.itemDataScriptable.descriptions[item];

            MenuManager_old.instance.drawMenu.items[i] = item;
            MenuManager_old.instance.drawMenu.boxVisuals[i].description.text = desc;
            MenuManager_old.instance.drawMenu.boxVisuals[i].name.text = name;
            MenuManager_old.instance.drawMenu.boxVisuals[i].icon.sprite = icon;
            MenuManager_old.instance.drawMenu.isMenuActive = true;
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
            for (int i = 0; i < ObjectsManager_old.instance.itemList.Count; i++)
            {
                //add every possible item to the list
                doNotChooseTheSameObjectList.Add(ObjectsManager_old.instance.itemList[i]);
            }
            //actives a canvas to choose 2 objects from
            //assigns box object with a random item
            item1 = doNotChooseTheSameObjectList[Random.Range(0, doNotChooseTheSameObjectList.Count)];
            doNotChooseTheSameObjectList.Remove(item1);
            //updates icon
            spriteRenderer1.sprite = ObjectsManager_old.instance.objectSprites[item1];
        }

        else if (numberOfDrops == 2)
        {
            //sets both loots
            //sets item
            List<int> doNotChooseTheSameObjectList = new List<int>();
            for (int i = 0; i < ObjectsManager_old.instance.itemList.Count; i++)
            {
                //add every possible item to the list
                doNotChooseTheSameObjectList.Add(ObjectsManager_old.instance.itemList[i]);
            }
            //actives a canvas to choose 2 objects from
            //assigns box object with a random item
            item1 = doNotChooseTheSameObjectList[Random.Range(0, doNotChooseTheSameObjectList.Count)];
            doNotChooseTheSameObjectList.Remove(item1);
            item2 = doNotChooseTheSameObjectList[Random.Range(0, doNotChooseTheSameObjectList.Count)];
            doNotChooseTheSameObjectList.Remove(item2);
            //updates icon
            spriteRenderer1.sprite = ObjectsManager_old.instance.objectSprites[item1];
            spriteRenderer2.sprite = ObjectsManager_old.instance.objectSprites[item2];
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
