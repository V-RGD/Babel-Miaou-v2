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

    public GameManager gameManager;
    

    private void Awake()
    {
        playerControls = new PlayerControls();
        player = GameObject.Find("Player");
        itemEffect = GetComponent<ItemEffect>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        messagePrompt.GetComponent<TMP_Text>().text = cost.ToString() + " noeuils";
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
        if ((player.transform.position - transform.position).magnitude <= grabDist)
        {
            //show message prompt
            isPlayerInRange = true;
            messagePrompt.SetActive(true);
        }
        else
        {
            //disable message prompt
            isPlayerInRange = false;
            messagePrompt.SetActive(false);
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
