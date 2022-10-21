using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject player;
    public GameObject[] items;
    private GameVariables _gameVariables;
    private UIManager _uiManager;
    private ObjectsManager _objectsManager;
    private PlayerController _playerController;
    private Room _room;

    public int money;
    public int maxHealth = 3;

    public int health;

    public int healthBonus;

    public int currentRoom;
    public bool isDead;

    private void Awake()
    {
        player = GameObject.Find("Player");
        _objectsManager = GetComponent<ObjectsManager>();
        _gameVariables = GetComponent<ObjectsManager>().gameVariables;
        _playerController = player.GetComponent<PlayerController>();
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    void Start() => health = maxHealth;

    private void Update()
    {
        //caps health to the max amount
        if (maxHealth < health)
        {
            health = maxHealth;
        }

        //ded
        if (health <= 0)
        {
            if (_objectsManager.catLuck)
            {
                int canRes = Random.Range(0, 100);
                if (canRes <= _gameVariables.catLuckResRate)
                {
                    //destroys catnip
                    for (int i = 0; i < 5; i++)
                    {
                        GameObject item = _objectsManager.itemObjectsInventory[i];
                        if (item.GetComponent<ItemDragDrop>().objectID == 22)
                        {
                            _objectsManager.itemObjectsInventory[i] = null;
                            Destroy(item);
                        }
                    }
                    //adds health
                    health = Mathf.CeilToInt(_gameVariables.catLuckResHp);
                }
            }
            else
            {
                isDead = true;
            }
        }
    }
}
