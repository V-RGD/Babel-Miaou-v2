using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    private GameVariables _gameVariables;
    private UIManager _uiManager;
    private ObjectsManager _objectsManager;
    private PlayerController _playerController;
    private Room _room;
    [HideInInspector]public CinemachineShake cmShake;
    public List<Transform> eyesInGame;
    private EnemyHitFx _enemyHitFx;
    public bool hardcoreMode;

    public int money;
    public int maxHealth = 3;
    public int health;
    public float enemyHitShakeIntensity = 3;

    public int playerRoom;
    public bool isDead;
    private bool isFreezed;
    public int currentLevel;

    public Material hurtRenderMat;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        
        cmShake = GameObject.Find("TestCam").GetComponent<CinemachineShake>();
        _enemyHitFx = GetComponent<EnemyHitFx>();
    }

    void Start()
    {
        _objectsManager = ObjectsManager.instance;
        _gameVariables = _objectsManager.gameVariables;
        _playerController = PlayerController.instance;
        _uiManager = UIManager.instance;
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

    public void DealDamageToPlayer(float damageDealt) //when player takes hit
    {
        if (_playerController.invincibleCounter <= 0 && _objectsManager.sacredCrossTimer <= 0 && !_playerController.isDashing)
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
            cmShake.ShakeCamera(7, .1f);
            
            hurtRenderMat.SetFloat("Strenght",  (1 - ((float) health / (float) maxHealth)));
            Debug.Log((1 - ((float) health / (float) maxHealth))); 
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
                    ObjectsManager.instance.PlayActivationVfx(4);
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
        EnemyVfx.instance.hitFx.StartCoroutine(EnemyVfx.instance.hitFx.PlaceNewVfx(EnemyVfx.instance.hitFx.particleList[0], enemy.transform.position, true));
        EnemyVfx.instance.hitFx.StartCoroutine(EnemyVfx.instance.hitFx.PlaceNewVfx(EnemyVfx.instance.hitFx.particleList[1], enemy.transform.position, true));
        EnemyVfx.instance.hitFx.StartCoroutine(EnemyVfx.instance.hitFx.PlaceNewVfx(EnemyVfx.instance.hitFx.particleList[2], enemy.transform.position, true));
        _enemyHitFx.StartCoroutine(_enemyHitFx.PlaceNewVfx(enemy.transform.position));
        //clamps damage to an int (security)
        float damage = damageDealt;
        //applies damage
        if (_objectsManager.killingSpreeTimer > 0)
        {
            damage *= _gameVariables.killingSpreeDamage;
            ObjectsManager.instance.PlayActivationVfx(0);
        }
        //if no hit, doubles damage
        if (_objectsManager.noHitStreak)
        {
            damage *= 1.3f;
            ObjectsManager.instance.PlayActivationVfx(6);
        }
        enemy.health -= damage;
        //Debug.Log(damage);
        cmShake.ShakeCamera(enemyHitShakeIntensity, .1f);
        //applies killing effects
        if (enemy.health <= 0)
        {
            _objectsManager.OnEnemyKill();
            GameScore.instance.AddScore(30);
            enemy.Death();
        }
        enemy.SliderUpdate();
        GameScore.instance.AddScore(10);
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
