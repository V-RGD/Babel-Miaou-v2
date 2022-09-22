using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopItem : MonoBehaviour
{
    private bool isPlayerInRange;
    private float grabDist = 5;

    public int cost;
    
    public PlayerControls playerControls;
    public InputAction collect;

    public GameObject player;
    public ItemEffect itemEffect;
    public GameObject messagePrompt;
    public GameObject collectPrompt;

    public GameManager gameManager;
    public bool isFromAShop;
    

    private void Awake()
    {
        playerControls = new PlayerControls();
        player = GameObject.Find("Player");
        itemEffect = GetComponent<ItemEffect>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        messagePrompt.GetComponent<TMP_Text>().text = cost.ToString() + " noeuils";
        messagePrompt.SetActive(false);
        collectPrompt.SetActive(false);
    }

    void Collect(InputAction.CallbackContext context)
    {
        if (isPlayerInRange && gameManager.money >= cost)
        {
            //does gameobject effect
            itemEffect.activeEffect = true;
            gameManager.money -= cost;
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        
        //check whether the item is from the shop or not
        if (!isFromAShop)
        {
            //can collect just with a press
            cost = 0;
        }
        
        if ((player.transform.position - transform.position).magnitude <= grabDist)
        {
            //show message prompt
            isPlayerInRange = true;
            
            if (isFromAShop)
            {
                messagePrompt.SetActive(true);
            }
            else
            {
                collectPrompt.SetActive(true);
            }
        }
        else
        {
            //disable message prompt
            isPlayerInRange = false;
            
            if (isFromAShop)
            {
                messagePrompt.SetActive(false);
            }
            else
            {
                collectPrompt.SetActive(false);
            }
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
