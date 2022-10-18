using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsManager : MonoBehaviour
{
    
    public List<GameObject> itemObjectsInventory = new List<GameObject>(5);
    public GameObject[] itemList;
    public ObjectTextData itemDataScriptable;
    public Sprite[] objectSprites;

    //creates special pools
    [HideInInspector]public List<GameObject> shopPool;
    [HideInInspector]public List<GameObject> chestPool;
    [HideInInspector]public List<GameObject> specialChestPool;

    public GameObject[] uiItemBoxes;
    public GameObject objectMenu;

    public GameObject uiItemPrefab;

    private GameVariables _gameVariables;
    
    //objects
    public bool allSeeingEye; //actives full map
    public bool bloodBlade; //killing x enemies rewards y health
    public bool foreignFriend; //a random enemy is killed when entering a room.
    public bool glassCanon; //each xHP lost, gains yAtk;
    public bool catWrath; //if hit, attack multiplied by x, attack cooldown reduced by y, during z seconds
    public bool allKnowingEye; //highlights interesting places on the map
    public bool assassin; //backstabbing deals 1.5x dmg
    public bool killingSpree; //killing an enemy grants X extra damage during Y seconds. Dash is cooldown is reset
    
    public bool theForce; //small attacks repels projectiles
    public bool masterSword; //if full hp, small attacks launches projectiles dealing 50% damage
    public bool sacredCross; 
    public bool bluSmash;
    public bool innerPeace;
    public bool noPetting;
    public bool witherShield;
    
    public bool strongGrasp;
    public bool swiftArt;
    public bool tankPower;
    public bool noHit;
    public bool strangePact;
    
    public GameObject knittingBall;
    public GameObject eyeCollector;
    public bool catLuck;
    public bool catNip;
    public bool safetyBlessing;

    private int _bsbKillStreak;
    private bool _ffCanKill;
    private GameManager _gameManager;
    private UIManager _uiManager;
    private PlayerController _player;

    public bool playerHit;

    private float _catWrath_damageMultiplier;
    private float _catWrath_attackCooldown;
    private float _invincibilityTimer;
    private float _sacredCross_tookDamageTimer;
    private float _witherShield_speedDecrease;
    private float _glassCanonDamage;
    private float _catWrathTimer;
    private float _killingSpreeTimer;
    public int sourceDamage;
    private int _currentRoomIndex;
    private bool noHitStreak;
    public bool enteredRoom;
    public bool killedEnemy;
    private Room _currentRoom;

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    private void Start()
    {
        AssignObjectInfos();
        CreateItemPools();
    }

    private void Update()
    {
        UiItemBoxesUpdate();
    }

    private void FixedUpdate()
    {
        ObjectEffects();
    }

    public void OnObjectEquip(GameObject item)
    {
        int id = item.GetComponent<ItemDragDrop>().objectID;
        Debug.Log("equiped " + id);
        //check the ID of the object to add additional effects
        switch (id)
        {
            case 0 : allSeeingEye = true; break;
            case 1 : bloodBlade = true; break;
            case 2 : foreignFriend = true; break;
            case 3 : glassCanon = true; break;
            case 4 : catWrath = true; break;
            case 5 : allKnowingEye = true; break;
            case 6 : assassin = true; break;
            case 7 : killingSpree = true; break;
            case 8 : theForce = true; break;
            case 9 : masterSword = true; break;
            case 10 : sacredCross = true; break;
            case 11 : bluSmash = true; break;
            case 12 : innerPeace = true; break;
            case 13 : noPetting = true; break;
            case 14 : witherShield = true; break;
            case 15 : strongGrasp = true; break;
            case 16 : swiftArt = true; break;
            case 17 : tankPower = true; break;
            case 18 : noHit = true; break;
            case 19 : strangePact = true; break;
            case 20 : knittingBall.SetActive(true); break;
            case 21 : eyeCollector.SetActive(true); break;
            case 22 : catLuck = true; break;
            case 23 : catLuck = true; break;
            case 24 : allSeeingEye = true; break;
            case 25 : allSeeingEye = true; break;
        }
    }
    public void OnObjectUnEquip(GameObject item)
    {
        int id = item.GetComponent<ItemDragDrop>().objectID;
        Debug.Log("unequiped " + id);
        //check the ID of the object to remove additional effects
        switch (id)
        {
            case 0 : allSeeingEye = false; break;
            case 1 : bloodBlade = false; break;
            case 2 : foreignFriend = false; break;
            case 3 : glassCanon = false; break;
            case 4 : catWrath = false; break;
            case 5 : allKnowingEye = false; break;
            case 6 : assassin = false; break;
            case 7 : killingSpree = false; break;
            case 8 : theForce = false; break;
            case 9 : masterSword = false; break;
            case 10 : sacredCross = false; break;
            case 11 : bluSmash = false; break;
            case 12 : innerPeace = false; break;
            case 13 : noPetting = false; break;
            case 14 : witherShield = false; break;
            case 15 : strongGrasp = false; break;
            case 16 : swiftArt = false; break;
            case 17 : tankPower = false; break;
            case 18 : noHit = false; break;
            case 19 : strangePact = false; break;
            case 20 : knittingBall.SetActive(false); break;
            case 21 : eyeCollector.SetActive(false); break;
            case 22 : catLuck = false; break;
            case 23 : catLuck = false; break;
            case 24 : allSeeingEye = false; break;
            case 25 : allSeeingEye = false; break;
        }
    }

    void ObjectEffects()
    {
        //--------------------Permanent Effects---------------------------------

        //all seeing eye - glass canon - all knowing eye - the force - master sword - no pet - strong grasp - swift art - tank power - strange pact - catnip - safety blessing
        switch (allSeeingEye)
        {
            case true : _uiManager.isMapFull = true; break; //sets map to full size
            case false : _uiManager.isMapFull = false; break;
        }
        switch (glassCanon)
        {
            case true : //increases damage depending on the effect wished
                int maxHealth = _gameManager.maxHealth;
                int health = _gameManager.health;
                int diff = (maxHealth - health) % 2; //rounds attack to int
                switch (diff)
                {
                    case 0 : _glassCanonDamage = (maxHealth - health) / 2 * _gameVariables.glassCanonDamage; break; 
                    case 1 : _glassCanonDamage = Mathf.CeilToInt((maxHealth - health) / 2 * _gameVariables.glassCanonDamage); break;
                }
                break;
            case false : break;
        }
        switch (allKnowingEye)
        {
            case true : _uiManager.isMapHighlight = true; break; //highlights interesting places on the map
            case false : _uiManager.isMapHighlight = false; break;
        }
        switch (theForce)
        {
            case true : _player.canRepel = true; break;
            case false : _player.canRepel = false; break;
        }
        switch (masterSword)
        {
            case true : _player.isMasterSword = true; break;
            case false : _player.isMasterSword = false; break;
        }
        //--------------------Event Effects---------------------------------
        
        //on hit - cat wrath - sacred cross - inner peace - wither shield - no hit
        if (playerHit)
        {
            playerHit = false;
            if (catWrath) 
            {
                _catWrathTimer = _gameVariables.catWrathTime; //sets timer
            }
            if (sacredCross)
            {
                _player.invincibleCounter = _gameVariables.sacredCrossTimer;
            }
            if (innerPeace) 
            {
                int damage = sourceDamage;
                if (damage != 1)
                {
                    damage--;
                }
                _gameManager.health -= damage;
            }

            if (witherShield)
            {
                List<GameObject> enemyList = new List<GameObject>();
                for (int i = 0; i < _currentRoom.enemyGroup.transform.childCount; i++)
                {
                    //access enemy speed and decreases it
                    //_currentRoom.enemyGroup.transform.GetChild(i).GetComponent<Enemy>().speedFactor = _gameVariables.witherShield_speedDecrease;
                }
            }

            switch (noHit)
            {
                case true : noHitStreak = false; break;
            }
        }
        
        //--------------------other---------------------------------
        //knit ball - eye collector - catluck - 
        
        switch (bloodBlade)
        {
            case true :
                if (_bsbKillStreak == 10)
                {
                    _gameManager.health += 1;
                    _bsbKillStreak = 0;
                }
                break;
            case false : _bsbKillStreak = 0; break;
        }
        switch (foreignFriend)
        {
            case true :
                if (_ffCanKill && enteredRoom)
                {
                    enteredRoom = false;
                    //kills random enemy
                }
                break;
            case false :  break;
        }
        switch (assassin) //later
        {
            case true : break;  //if hits on the back, deals extra damage
            case false : break;
        }
        if (killedEnemy) //when kills an enemy
        {
            killedEnemy = false;
            if (killingSpree)
            {
                _killingSpreeTimer = _gameVariables.killingSpreeTimer; //sets timer
                _player.dashCooldownTimer = 0;
            }
        }
        
        switch (bluSmash) //later
        {
            case true : break;
            case false : break;
        }

        //---------Manages effects based on timers-------------
        if (_catWrathTimer > 0)
        {
            _catWrath_damageMultiplier = _gameVariables.catWrath_damageMultiplier; //increases attack speed and damage
            _catWrath_attackCooldown = _gameVariables.catWrath_attackCooldown;
        }
    }

    void StatsCalculation()
    {
        //attack
        //dex
        //max health
    }
    void CreateItemPools()
    {
        //chest pool = reserved chest + commom
        foreach (var id in itemDataScriptable.chestReservedItems)
        {
            chestPool.Add(itemList[id]);
        }
        
        foreach (var id in itemDataScriptable.commonItems)
        {
            chestPool.Add(itemList[id]);
        }

        //special chest pool = special chest
        foreach (var id in itemDataScriptable.specialChestReservedItems)
        {
            specialChestPool.Add(itemList[id]);
        }
        
        //shop pool
        foreach (var id in itemDataScriptable.shopItemReservedItems)
        {
            shopPool.Add(itemList[id]);
        }
        foreach (var id in itemDataScriptable.commonItems)
        {
            shopPool.Add(itemList[id]);
        }
    }
    void AssignObjectInfos()
    {
        //assigns object info depending on it's position on the list.
        for (int i = 0; i < itemList.Length; i++)
        {
            itemList[i].GetComponent<Item>().objectID = i;
            itemList[i].GetComponent<Item>().description = itemDataScriptable.descriptions[i];
            itemList[i].GetComponent<Item>().itemName = itemDataScriptable.names[i];
            itemList[i].GetComponent<Item>().rarity = itemDataScriptable.rarity[i];
            switch (itemList[i].GetComponent<Item>().rarity)
            {
                case 1 : itemList[i].GetComponent<Item>().itemCost = 15; break;
                case 2 : itemList[i].GetComponent<Item>().itemCost = 25; break;
                case 3 : itemList[i].GetComponent<Item>().itemCost = 35; break;
                case 4 : itemList[i].GetComponent<Item>().itemCost = 35; break;
            }
            itemList[i].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = objectSprites[i];
        }
    }

    void UiItemBoxesUpdate()
    {
        //used to reload data from the object when added
        for (int i = 0; i < 6; i++)
        {
            if (itemObjectsInventory[i] != null)
            {
                int id = itemObjectsInventory[i].GetComponent<ItemDragDrop>().objectID;
                //update : box name, icon, description, rarity color
                string name = itemDataScriptable.names[id];
                Sprite icon = objectSprites[id];
                string desc = itemDataScriptable.descriptions[id];
                int rarity = itemDataScriptable.rarity[id];
                Color color = Color.grey;

                uiItemBoxes[i].transform.GetChild(1).GetComponent<TMP_Text>().text = name;
                //uiItemBoxes[i].transform.GetChild(3).GetComponent<Image>().sprite = icon;
                uiItemBoxes[i].transform.GetChild(2).GetComponent<TMP_Text>().text = desc;
                switch (rarity)
                {
                    case 1 : color = Color.green; break;
                    case 2 : color = Color.blue; break;
                    case 3 : color = Color.magenta; break;
                    case 4 : color = Color.yellow; break;
                }
                uiItemBoxes[i].transform.GetChild(0).GetComponent<Image>().color = color;
            }
            else
            {
                //shows empty box
                uiItemBoxes[i].transform.GetChild(1).GetComponent<TMP_Text>().text = "<Add Module>";
                //uiItemBoxes[i].transform.GetChild(4).GetComponent<Image>().sprite = null;
                uiItemBoxes[i].transform.GetChild(2).GetComponent<TMP_Text>().text = "";
                uiItemBoxes[i].transform.GetChild(0).GetComponent<Image>().color = Color.grey;
            }
        }
    }
}
