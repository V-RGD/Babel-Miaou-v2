using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsManager : MonoBehaviour
{
    public int itemAmount;
    public GameObject objectTemplate;

    public List<GameObject> itemObjectsInventory = new List<GameObject>(5);
    public List<GameObject> itemList;
    public ObjectTextData itemDataScriptable;
    public Sprite[] objectSprites;

    //creates special pools
    [HideInInspector]public List<GameObject> shopPool;
    [HideInInspector]public List<GameObject> chestPool;
    [HideInInspector]public List<GameObject> specialChestPool;

    public GameObject[] uiItemBoxes;
    public GameObject objectMenu;
    public GameObject uiItemPrefab;
    public GameObject knittingBall;
    public GameObject eyeCollector;
    public GameObject healToken;
    public GameObject eyeToken;

    private bool bloodBlade; //killing x enemies rewards y health
    public bool foreignFriend; //a random enemy is killed when entering a room.
    private bool sacredCross; //invincible time when hit
    private bool bluSmash;
    private bool innerPeace;
    private bool killingSpree; //killing an enemy grants X extra damage during Y seconds. Dash is cooldown is reset
    private bool strongGrasp;
    private bool swiftArt;

    private bool noHit; //actives when entering a room - player gets 2x damage until hit
    public bool strangePact; //can pay with life instead of eyes
    public bool catLuck;
    public bool safetyBlessing; //when player hit by a projectile, chances are that it won't hit
    private bool witherShield; //slows down enemies on hit

    //attack
    float glassCanonDamage;
    float catWrathDamageMultiplier;
    float assassinDamageMultiplier;
    float killingSpreeDamage;
    float tankPowerDamage;
    float noHitSpeedRunDamageMultiplier;
    float catNipDamage;

    float catNipHpIncrease;
    float catWrathDexterityIncrease;
    float catNipDexIncrease;
    float catNipSpeedIncrease;

    private GameManager _gameManager;
    private UIManager _uiManager;
    private PlayerController _player;
    private Room _currentRoom;
    public GameVariables gameVariables;
    
    public float catWrathTimer;
    public float witherShieldTimer;
    public float killingSpreeTimer;
    public float sacredCrossTimer;
    
    public bool noHitStreak;
    private int _bsbKillStreak;
    private bool _ffCanKill;
    
    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void Start()
    {
        GameObject littleShit = Instantiate(gameVariables.eyeCollector);
        GameObject knitBall = Instantiate(gameVariables.knittingBall);
        eyeCollector = littleShit;
        knittingBall = knitBall;
        AssignObjectInfos();
        CreateItemPools();

        _player.dexterity = gameVariables.baseDexterity;
        _player.maxSpeed = gameVariables.baseSpeed;
        _player.attackStat = gameVariables.baseAttack;
    }
    
    public void OnObjectEquip(GameObject item)
    {
        int id = item.GetComponent<ItemDragDrop>().objectID;
        Debug.Log("equiped " + id);
        //check the ID of the object to add additional effects
        switch (id)
        {
            case 0 : _uiManager.isMapFull = true; break; //sets map to full size
            case 1 : bloodBlade = true; break;
            case 2 : foreignFriend = true; break;
            case 3 : glassCanonDamage = gameVariables.glassCanonDamage; break;
            case 4 : catWrathDamageMultiplier = gameVariables.catWrathDamageMultiplier; catWrathDexterityIncrease = gameVariables.catWrathDexterityIncrease; break;
            case 5 : _uiManager.isMapHighlight = true; break; //highlights interesting places on the map
            case 6 : assassinDamageMultiplier = gameVariables.assassinDamageMultiplier; break;
            case 7 : killingSpree = true; break;
            case 8 : _player.canRepel = false; break;
            case 9 : _player.isMasterSword = true; break;
            case 10 : sacredCross = true; break;
            case 11 : bluSmash = true; break;
            case 12 : innerPeace = true; break;
            case 13 : _player.noPet = true; break;
            case 14 : witherShield = true; break;
            case 15 : strongGrasp = true; break;
            case 16 : swiftArt = true; break;
            case 17 : tankPowerDamage = gameVariables.tankPowerDamage; break;
            case 18 : noHit = true; break;
            case 19 : strangePact = true; break;
            case 20 : gameVariables.knittingBall.SetActive(true); break;
            case 21 : gameVariables.eyeCollector.SetActive(true); break;
            case 22 : catLuck = true; break;
            case 23 : catNipDamage = gameVariables.catNipDamage; catNipHpIncrease = gameVariables.catNipHpIncrease; catNipDexIncrease = gameVariables.catNipDexIncrease; catNipSpeedIncrease = gameVariables.catNipSpeedIncrease; break; break;
            case 24 : safetyBlessing = true; break;
        }
        UiItemBoxesUpdate();
        UpdateStats();
    }
    public void OnObjectUnEquip(GameObject item)
    {
        int id = item.GetComponent<ItemDragDrop>().objectID;
        Debug.Log("unequiped " + id);
        //check the ID of the object to remove additional effects
        switch (id)
        {
            case 0 : _uiManager.isMapFull = false; break; //sets map to normal size
            case 1 : bloodBlade = false; break;
            case 2 : foreignFriend = false; break;
            case 3 : glassCanonDamage = 0; break;
            case 4 : catWrathDamageMultiplier = 1; catWrathDexterityIncrease = 0; break;
            case 5 : _uiManager.isMapHighlight = false; break; //highlights interesting places on the map
            case 6 : assassinDamageMultiplier = 1; break;
            case 7 : killingSpree = false; break;
            case 8 : _player.canRepel = false; break;
            case 9 : _player.isMasterSword = false; break;
            case 10 : sacredCross = false; break;
            case 11 : bluSmash = false; break;
            case 12 : innerPeace = false; break;
            case 13 : _player.noPet = false; break;
            case 14 : witherShield = false; break;
            case 15 : strongGrasp = false; break;
            case 16 : swiftArt = false; break;
            case 17 : tankPowerDamage = 0; break;
            case 18 : noHit = false; break;
            case 19 : strangePact = false; break;
            case 20 : knittingBall.SetActive(false); break;
            case 21 : eyeCollector.SetActive(false); break;
            case 22 : catLuck = false; break;
            case 23 : catNipDamage = 0; catNipHpIncrease = 0; catNipDexIncrease = 0; catNipSpeedIncrease = 0; break;
            case 24 : safetyBlessing = false; break;
        }
        UiItemBoxesUpdate();
        UpdateStats();
    }
    void UpdateStats()
    {
        //glass canon
        int diff = (_gameManager.maxHealth - _gameManager.health) % 2; //rounds attack to int
        switch (diff)
        {
            case 0 : glassCanonDamage = (_gameManager.maxHealth - _gameManager.health) / gameVariables.glassCanonHealthNeeded * gameVariables.glassCanonDamage; break; 
            case 1 : glassCanonDamage = Mathf.CeilToInt((_gameManager.maxHealth - _gameManager.health) / gameVariables.glassCanonHealthNeeded * gameVariables.glassCanonDamage); break;
        }
        glassCanonDamage = 0;

        //if took damage, multiplies attack by 50%, , diminishes attack cooldown
        if (catWrathTimer > 0)
        {
            catWrathDamageMultiplier = gameVariables.catWrathDamageMultiplier; 
            catWrathDexterityIncrease = gameVariables.catWrathDexterityIncrease;
        }
        else
        {
            catWrathDamageMultiplier = 1; 
            catWrathDexterityIncrease = 0; 
        }

        
        //killing spree
        if (killingSpreeTimer > 0)
        {
            //if just killed an enemy, increases damage
            killingSpreeDamage = gameVariables.killingSpreeDamage;
        }
        else
        {
            killingSpreeDamage = 0;
        }

        
        //tank power
        float tankPowerBonus;
        if (_gameManager.health > gameVariables.tankPowerCeiling)
        {
            //if hp greater than base stat, increases damage
            tankPowerBonus = tankPowerDamage * (_gameManager.health - gameVariables.tankPowerCeiling);
        }
        else
        {
            tankPowerBonus = 0;
        }
        
        if (noHitStreak)
        {
            noHitSpeedRunDamageMultiplier = gameVariables.noHitSpeedRunDamageMultiplier;
        }
        else
        {
            noHitSpeedRunDamageMultiplier = 1;
        }

        float attackBonuses = glassCanonDamage + killingSpreeDamage + tankPowerBonus + catNipDamage;
        float attack = (gameVariables.baseAttack + attackBonuses) * noHitSpeedRunDamageMultiplier * assassinDamageMultiplier * catWrathDamageMultiplier;
        _player.attackStat = attack;
        
        //HP Max
        _gameManager.maxHealth = Mathf.CeilToInt(gameVariables.baseHealth + catNipHpIncrease + _gameManager.healthBonus);

        //dexterity
        float bonusDex = catWrathDexterityIncrease + catNipDexIncrease;
        float dex = (gameVariables.baseDexterity + bonusDex);
        _player.dexterity = dex;

        float speedBonus = catNipSpeedIncrease;
        float speed = gameVariables.baseSpeed + speedBonus;
        _player.maxSpeed = speed;

        //Debug.Log("dex set to " + dex);
        //Debug.Log("speed set to " + speed);
        //Debug.Log("attack set to " + attack);
        /*
        Debug.Log("health set to " + _gameManager.maxHealth);
        Debug.Log("dex set to " + dex);
        Debug.Log("speed set to " + speed);
        Debug.Log("attack set to " + attack);*/
    }

    public void OnEnemyKill()
    {
        if (bloodBlade)
        {
            //adds  to the kill counter
            _bsbKillStreak++;
            //if kill counter = ceil, rewards player with hp (bsb)
            if (_bsbKillStreak >= gameVariables.bsbEnemiesNeeded)
            {
                _bsbKillStreak = 0;
                _gameManager.health += Mathf.FloorToInt(gameVariables.bsbHealthReward);
            }
        }
        
        if (killingSpree)
        {
            _player.dashCooldownTimer = 0;
            killingSpreeTimer = gameVariables.killingSpreeLength;
        }
    }

    public void OnPlayerHit(int sourceDamage)
    {
        int damage = sourceDamage;
        catWrathTimer = gameVariables.catWrathLength; //sets timer
        //on hit - cat wrath - sacred cross - inner peace - wither shield - no hit
        if (sacredCross)
        {
            _player.GetComponent<PlayerController>().invincibleCounter = gameVariables.sacredCrossLength;
        }
        if (innerPeace)
        {
            if (damage - 1 >= 1)
            {
                damage--;
            }
            else
            {
                damage = 1;
            }
        }

        if (witherShield)
        {
            for (int i = 0; i < _currentRoom.enemyGroup.transform.childCount; i++)
            {
                //access enemy speed and decreases it
                //_currentRoom.enemyGroup.transform.GetChild(i).GetComponent<Enemy>().speedFactor = gameVariables.witherShieldSlowAmount;
            }
        }

        if (noHit)
        {
            noHitStreak = false;
        }
        
        _gameManager.health -= damage;
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
        for (int i = 0; i < itemAmount; i++)
        {
            GameObject item = Instantiate(objectTemplate, transform);
            item.name = "Item#" + i;
            item.SetActive(false);
            itemList.Add(item);
        }
        
        //assigns object info depending on it's position on the list.
        for (int i = 0; i < itemList.Count; i++)
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
    public void UiItemBoxesUpdate()
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
