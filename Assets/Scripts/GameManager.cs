using System.Collections;
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
    public CinemachineShake _cmShake;
    private PlayerAttacks _playerAttacks;

    public int money;
    public int maxHealth = 3;
    public int health;
    public int healthBonus;

    public int currentRoom;
    public bool isDead;
    private bool isFreezed;

    private void Awake()
    {
        player = GameObject.Find("Player");
        _objectsManager = GetComponent<ObjectsManager>();
        _gameVariables = GetComponent<ObjectsManager>().gameVariables;
        _playerController = player.GetComponent<PlayerController>();
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        _cmShake = GameObject.Find("TestCam").GetComponent<CinemachineShake>();
        _playerAttacks = player.GetComponent<PlayerAttacks>();
    }

    void Start()
    {
        health = maxHealth;
        _uiManager.HealthBar(health);
    }

    IEnumerator FreezeFrame(float length)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(length);
        Time.timeScale = 0;
        isFreezed = false;
    }

    private void Update()
    {
        //caps health to the max amount
        if (health > maxHealth)
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
                        int item = _objectsManager.itemObjectsInventory[i];
                        if (item == 22)
                        {
                            //destroys item
                            _objectsManager.itemObjectsInventory[i] = 999;
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
    
    public void DealDamageToPlayer(float damageDealt) //when player takes hit
    {
        if (_playerController.invincibleCounter < 0)
        {
            //clamps damage to an int (security)
            int damage = Mathf.CeilToInt(damageDealt);
            //applies effects --- including damage taken
            _objectsManager.OnPlayerHit(damage);
            //player is invincible for a time
            _playerController.invincibleCounter = _playerController.invincibleTime;
            //sets health bar
            _uiManager.HealthBar(health);
            _playerController.invincibleCounter = 1;
            _cmShake.ShakeCamera(2, .1f);
        }
    }
    
    public void DealDamageToEnemy(float damageDealt, Enemy enemy) //when enemy takes hit
    {
        //clamps damage to an int (security)
        int damage = Mathf.CeilToInt(damageDealt);
        //applies killing effects
        if (enemy.health - damage <= 0)
        {
            _objectsManager.OnEnemyKill();
        }
        //applies damage
        enemy.health -= damage;
        _cmShake.ShakeCamera(5, .1f);
        
        switch (_playerAttacks.comboState)
        {
            case PlayerAttacks.ComboState.SimpleAttack:
                _cmShake.ShakeCamera(2, .1f);
                break;
            case PlayerAttacks.ComboState.ReverseAttack:
                _cmShake.ShakeCamera(2, .1f);
                break;
            case PlayerAttacks.ComboState.SpinAttack:
                _cmShake.ShakeCamera(2, .1f);
                break;
        }
    }
    
    public void DealDamageToEnemy(float damageDealt, Enemy enemy, float shakeValue, float shakeAmount) //when enemy takes hit
    {
        //clamps damage to an int (security)
        int damage = Mathf.CeilToInt(damageDealt);
        //applies killing effects
        if (enemy.health - damage <= 0)
        {
            _objectsManager.OnEnemyKill();
        }
        //applies damage
        enemy.health -= damage;
        _cmShake.ShakeCamera(5, .1f);
    }
}
