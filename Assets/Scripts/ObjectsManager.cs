using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ObjectsManager : MonoBehaviour
{
    public static ObjectsManager instance;
    
    #region Lists
    [Header("Lists")] [Space]
    public List<int> itemObjectsInventory = new List<int>(3);
    [SerializeField] public List<int> itemList;
    #endregion
    #region Prefabs
    [Header("Prefabs")] [Space]
    public GameObject objectTemplate; [Space]
    public GameObject uiItemPrefab; [Space]
    public GameObject eyeCollector;
    public GameObject eyeToken; [Space]
    public GameObject healItem;
    public GameObject randomItem;
    public GameObject maxHealthItem;
    #endregion
    #region Assignations
    [Header("Assignations")] [Space]
    [SerializeField] private Room _currentRoom;
    public GameObject[] uiItemBoxes;
    public GameObject[] uiActivationFx;
    private GameManager _gameManager;
    private PlayerControls _playerControls;
    private InputAction moveUp;
    private InputAction moveDown;
    private InputAction confirm;
    private UIManager _uiManager;
    private PlayerController _player;
    public GameObject objectMenu;
    public GameVariables gameVariables;
    public ObjectTextData itemDataScriptable;
    public Sprite[] objectSprites;
    public ParticleSystem sacredCrossFx;
    public ParticleSystem killingSpreeFx;
    public ParticleSystem noHitFx;
    public ParticleSystem resurrectionFx;
    #endregion
    #region Values
    [Header("Values")] [Space]
    public int itemAmount;
    #endregion
    #region Booleans
    //[HideInInspector] 
    public bool sacredCross; //invincible time when hit
    //[HideInInspector] 
    public bool killingSpree; //killing an enemy grants X extra damage during Y seconds. Dash is cooldown is reset
    //[HideInInspector] 
    public bool noHit; //actives when entering a room - player gets 2x damage until hit
    //[HideInInspector] 
    public bool eyeCollectorActive;
    public bool catLuck;
    //[HideInInspector] 
    public bool stinkyFish;
    //[HideInInspector] 
    public bool earthQuake;
    #endregion
    #region StatsIncrease
    private float killingSpreeDamage;
    #endregion
    #region Timers
    [HideInInspector] public float killingSpreeTimer;
    [HideInInspector] public float sacredCrossTimer;
    [HideInInspector] public bool noHitStreak;
    #endregion
    #region BoxMovement
    public int currentBoxPos;
    public float offsetDiff;
    public Vector3 bonusBoxStartPos;
    [HideInInspector]public bool canReplaceItem;
    #endregion
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }

        instance = this;
        
        _playerControls = new PlayerControls();
    }
    private void Start()
    {
        _player = PlayerController.instance;
        _uiManager = UIManager.instance;
        _gameManager = GameManager.instance;
        
        bonusBoxStartPos = uiItemBoxes[3].transform.position;

        PlayerAttacks.instance.dexterity = gameVariables.baseDexterity;
        _player.maxSpeed = gameVariables.baseSpeed;
        PlayerAttacks.instance.attackStat = gameVariables.baseAttack;

        for (int i = 0; i < uiActivationFx.Length - 1; i++)
        {
            uiActivationFx[i].SetActive(false);
        }
        AssignObjectInfos();
    }

    private void Update()
    {
        if (sacredCrossTimer > 0)
        {
            sacredCrossTimer -= Time.deltaTime;
        }
    }

    public void OnObjectEquip(int item)
    {
        itemList.Remove(item);
        Debug.Log("equiped Item#" + item);
        //check the ID of the object to add additional effects
        switch (item)
        {
            case 0 : killingSpree = true; break;
            case 1 : sacredCross = true; break;
            case 2 : stinkyFish = true; break;
            case 3 : eyeCollector.transform.position = _player.transform.position; eyeCollector.SetActive(true); eyeCollectorActive = true; break;
            case 4 : catLuck = true; break;
            case 5 : earthQuake = true; break;
            case 6 : noHit = true; break;
        }
        UiItemBoxesUpdate();
        //UpdateStats();
    }
    public void OnObjectUnEquip(int item)
    {
        if (item != 999)
        { 
            itemList.Add(item);
        }
        
        Debug.Log("unequiped Item#" + item);
        //check the ID of the object to remove additional effects
        switch (item)
        {
            case 0 : killingSpree = false; break;
            case 1 : sacredCross = false; break;
            case 2 : stinkyFish = false; break;
            case 3 : eyeCollector.SetActive(false);
                eyeCollectorActive = false; break;
            case 4 : catLuck = false; break;
            case 5 : earthQuake = false; break;
            case 6 : noHit = false; break;
        }
        UiItemBoxesUpdate();
    }
    public void OnEnemyKill()
    {
        if (killingSpree)
        {
            _player.dashCooldownTimer = 0;
            killingSpreeTimer = gameVariables.killingSpreeLength;
            killingSpreeFx.Play();
        }

        if (sacredCross)
        {
            sacredCrossTimer = gameVariables.sacredCrossLength;
            sacredCrossFx.Play();
        }
    }
    public void OnPlayerHit(int sourceDamage)
    {
        int damage = sourceDamage;
        //on hit - cat wrath - sacred cross - inner peace - wither shield - no hit
        if (sacredCross)
        {
            PlayerController.instance.invincibleCounter = gameVariables.sacredCrossLength;
        }

        if (noHit)
        {
            noHitStreak = false;
        }
        
        _gameManager.health -= damage;
    }
    void AssignObjectInfos()
    {
        for (int i = 0; i < 3; i++)
        {
            itemObjectsInventory[i] = 999;
        }
    }
    public void UiItemBoxesUpdate()
    {
        //used to reload data from the object when added
        for (int i = 0; i < 4; i++)
        {
            if (itemObjectsInventory[i] != 999)
            {
                int id = itemObjectsInventory[i];
                //update : box name, icon, description, rarity color
                string name = itemDataScriptable.names[id];
                Sprite icon = objectSprites[id];
                string desc = itemDataScriptable.descriptions[id];
                uiItemBoxes[i].transform.GetChild(1).GetComponent<TMP_Text>().text = name;
                uiItemBoxes[i].transform.GetChild(2).GetComponent<TMP_Text>().text = desc;
                uiItemBoxes[i].transform.GetChild(3).GetComponent<Image>().enabled = true;
                uiItemBoxes[i].transform.GetChild(3).GetComponent<Image>().sprite = icon;

                if (i < itemObjectsInventory.Count - 1)
                {
                    uiActivationFx[i].SetActive(true);
                }
            }
            else
            {
                //shows empty box
                uiItemBoxes[i].transform.GetChild(1).GetComponent<TMP_Text>().text = "<Empty Slot>";
                uiItemBoxes[i].transform.GetChild(2).GetComponent<TMP_Text>().text = "";
                uiItemBoxes[i].transform.GetChild(3).GetComponent<Image>().sprite = objectSprites[^1];
                uiItemBoxes[i].transform.GetChild(3).GetComponent<Image>().enabled = false;
                if (i < itemObjectsInventory.Count)
                {
                    uiActivationFx[i].SetActive(false);
                }
            }
        }
    }
    public void MoveExtraBox(int dir)
    {
        int pos;
        //if it goes beyond max
        if (currentBoxPos + dir > 2)
        {
            pos = 0;
        }
        //if it goes under min
        else if (currentBoxPos + dir < 0)
        {
            pos = 2;
        }
        else //alright
        {
            pos = currentBoxPos + dir;
        }
        
        //moves box and indent
        uiItemBoxes[3].transform.position = bonusBoxStartPos + Vector3.down * pos * offsetDiff;
        currentBoxPos = pos;
    }
    void MoveBoxUp(InputAction.CallbackContext context)
    {
        if (canReplaceItem)
        {
            MoveExtraBox(-1);
        }
    }
    void MoveBoxDown(InputAction.CallbackContext context)
    {
        if (canReplaceItem)
        {
            MoveExtraBox(1);
        }
    }
    void ReplaceItem(InputAction.CallbackContext context)
    {
        if (canReplaceItem)
        {
            //replaces item selected
            int oldItem = itemObjectsInventory[currentBoxPos];
            OnObjectUnEquip(oldItem);
            //adds new one
            int newItem = itemObjectsInventory[3];
            itemObjectsInventory[currentBoxPos] = newItem;
            OnObjectEquip(newItem);
            //destroys old item and empties 5th box
            uiItemBoxes[3].SetActive(false);
            canReplaceItem = false;
            MenuManager.instance.ObjectMenu();
        }
    }

    public void ReplaceItem(int box , int item)
    {
        //replaces item selected
        int oldItem = itemObjectsInventory[currentBoxPos];
        OnObjectUnEquip(oldItem);
        //adds new one
        int newItem = item;
        Debug.Log(box);
        itemObjectsInventory[box] = newItem;
        OnObjectEquip(newItem);
        _uiManager.UpdateHUDIcons();
        MenuManager.instance.ObjectMenu();
    }

    #region InputSystemRequirements
    private void OnEnable()
    {
        moveDown = _playerControls.UI.MoveDown;
        moveDown.Enable();
        moveDown.performed += MoveBoxDown;

        moveUp = _playerControls.UI.MoveUp;
        moveUp.Enable();
        moveUp.performed += MoveBoxUp;

        confirm = _playerControls.UI.Confirm;
        confirm.Enable();
        confirm.performed += ReplaceItem;
    }
    private void OnDisable()
    {
        moveDown.Disable();
        moveUp.Disable();
        confirm.Disable();
    }
    #endregion
}
