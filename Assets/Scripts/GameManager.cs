using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    private GameObject player;
    public GameObject[] items;
    private GameVariables _gameVariables;
    private UIManager _uiManager;
    private ObjectsManager _objectsManager;
    private PlayerController _playerController;
    private Room _room;
    public CinemachineShake _cmShake;
    [HideInInspector]public PlayerAttacks _playerAttacks;
    public List<Transform> eyesInGame;

    public int money;
    public int maxHealth = 3;
    public int health;
    public int healthBonus;

    public int playerRoom;
    public bool isDead;
    private bool isFreezed;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        instance = this;
        
        player = GameObject.Find("Player");
        _cmShake = GameObject.Find("TestCam").GetComponent<CinemachineShake>();
    }

    void Start()
    {
        _objectsManager = ObjectsManager.instance;
        _gameVariables = _objectsManager.gameVariables;
        _playerController = PlayerController.instance;
        _uiManager = UIManager.instance;
        _playerAttacks = PlayerAttacks.instance;
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
        
    }
    
    public void DealDamageToPlayer(float damageDealt) //when player takes hit
    {
        if (_playerController.invincibleCounter <= 0 && _objectsManager.sacredCrossTimer <= 0)
        {
            //clamps damage to an int (security)
            int damage = Mathf.CeilToInt(damageDealt);
            //applies effects --- including damage taken
            _objectsManager.OnPlayerHit(damage);
            //player is invincible for a time
            _playerController.invincibleCounter = _playerController.invincibleTime;
            //sets health bar
            _uiManager.HealthBar(health);
            _uiManager.HurtPanels();
            _playerController.invincibleCounter = 1;
            _cmShake.ShakeCamera(8, .1f);
            
            //ded
            if (health <= 0)
            {
                if (_objectsManager.catLuck)
                {
                    //destroys catnip
                    for (int i = 0; i < 3; i++)
                    {
                        if (_objectsManager.itemObjectsInventory[i] == 4)
                        {
                            //destroys item
                            _objectsManager.OnObjectUnEquip(4);
                            //adds new one
                            _objectsManager.itemObjectsInventory[i] = 999;
                            _uiManager.UpdateHUDIcons();
                        }
                    }
                    //adds health
                    health = Mathf.CeilToInt(_gameVariables.catLuckResHp);
                    _uiManager.HealthBar(health);
                }
                else
                {
                    isDead = true;
                }
            }
        }
    }
    
    public void DealDamageToEnemy(float damageDealt, Enemy enemy) //when enemy takes hit
    {
        //plays vfx
        enemy.splashFX.gameObject.SetActive(true);
        enemy.splashFX.Play();
        //clamps damage to an int (security)
        int damage = Mathf.CeilToInt(damageDealt);
        //applies damage
        if (_objectsManager.killingSpreeTimer > 0)
        {
            damage++;
        }
        enemy.health -= damage;
        _cmShake.ShakeCamera(5, .1f);
        //applies killing effects
        if (enemy.health <= 0)
        {
            _objectsManager.OnEnemyKill();
            enemy.Death();
        }
        enemy.SliderUpdate();
    }
    
    public void DealDamageToEnemy(float damageDealt, Enemy enemy, bool doShake) //when enemy takes hit
    {
        //clamps damage to an int (security)
        int damage = Mathf.CeilToInt(damageDealt);
        enemy.health -= damage;
        //applies killing effects
        if (enemy.health <= 0)
        {
            _objectsManager.OnEnemyKill();
            enemy.Death();
        }
        enemy.SliderUpdate();
    }
}
