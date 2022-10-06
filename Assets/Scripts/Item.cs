using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Item : MonoBehaviour
{
    private PlayerControls playerControls;
    private ObjectsManager _objectsManager;
    [HideInInspector]public InputAction collect;
    private GameManager gameManager;

    private GameObject player;
    public GameObject costPrompt;

    [HideInInspector]public bool isFromAShop;
    private bool isPlayerInRange;
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
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
        costPrompt.GetComponent<TMP_Text>().text = itemCost + " noeuils";
        costPrompt.SetActive(false);
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
        if (other.CompareTag("Player") && !isFromAShop)
        {
            //actives ui
            _objectsManager.itemBoxesUI.SetActive(true);
            //stop time
            Time.timeScale = 0;
            //actives choosing cursor : default on right, a cursor is placed and moves when pressing a button
            pressing s
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
