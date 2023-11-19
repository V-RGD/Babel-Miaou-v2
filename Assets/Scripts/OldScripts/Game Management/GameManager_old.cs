using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager_old : MonoBehaviour
{
    public static GameManager_old instance;
    
    private GameVariables _gameVariables;
    private UIManager _uiManager;
    private ObjectsManager_old _objectsManager;
    private PlayerController__old _playerController;
    private Room _room;
    [HideInInspector]public CinemachineShake cmShake;
    public List<Transform> eyesInGame;
    private EnemyHitFx _enemyHitFx;
    public bool hardcoreMode;

    public int money;
    public int maxHealth = 3;
    public int health = 10;
    public int initialMaxHealth;
    public float enemyHitShakeIntensity = 3;

    public int playerRoom;
    public bool isDead;
    private bool isFreezed;
    public int currentLevel;
    public float aberrationTimer;

    public Material hurtRenderMat;
    public ParticleSystem healFx;
    public ParticleSystem maxHpFx;
    public Animator healthBarAnimator;
    public Animator playerColorAnimator;

    public GameObject[] globalVolumes;
    public GameObject[] lights;
    public Animator hurtVolume;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        cmShake = GameObject.Find("TestCam").GetComponent<CinemachineShake>();
        _enemyHitFx = GetComponent<EnemyHitFx>();
    }

    void Start()
    {
        _objectsManager = ObjectsManager_old.instance;
        _gameVariables = _objectsManager.gameVariables;
        _playerController = PlayerController__old.instance;
        _uiManager = UIManager.instance;
        health = maxHealth;
        _uiManager.HealthBar(health);
        initialMaxHealth = maxHealth;
        hurtRenderMat.SetFloat("_Strenght",  0);
    }

    private void Update()
    {
        
    }

    public IEnumerator ShakeCam(int apex)
    {
        for (int i = 0; i < 5; i++)
        {
            cmShake.ShakeCamera(apex - i, .1f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator FreezeFrame(float length)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(length);
        Time.timeScale = 1;
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
            healthBarAnimator.CrossFade("Shake", 0);
            playerColorAnimator.CrossFade("Hurt", 0);
            hurtVolume.CrossFade("Hurt", 0);
            StartCoroutine(ShakeCam(3));
            
            hurtRenderMat.SetFloat("_Strenght",  (1 - ((float) health / (float) maxHealth)));
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
                    ObjectsManager_old.instance.PlayActivationVfx(4);
                }
                else
                {
                    isDead = true;
                    hurtRenderMat.SetFloat("_Strenght",  0);
                }
            }
        }
    }
    public void DealDamageToEnemy(float damageDealt, Enemy_old enemyOld) //when enemy takes hit
    {
        //plays vfx
        EnemyVfx.instance.hitFx.StartCoroutine(EnemyVfx.instance.hitFx.PlaceNewVfx(EnemyVfx.instance.hitFx.particleList[0], enemyOld.transform.position, true));
        EnemyVfx.instance.hitFx.StartCoroutine(EnemyVfx.instance.hitFx.PlaceNewVfx(EnemyVfx.instance.hitFx.particleList[1], enemyOld.transform.position, true));
        EnemyVfx.instance.hitFx.StartCoroutine(EnemyVfx.instance.hitFx.PlaceNewVfx(EnemyVfx.instance.hitFx.particleList[2], enemyOld.transform.position, true));
        _enemyHitFx.StartCoroutine(_enemyHitFx.PlaceNewVfx(enemyOld.transform.position));
        //clamps damage to an int (security)
        float damage = damageDealt;
        //applies damage
        if (_objectsManager.killingSpreeTimer > 0)
        {
            damage *= _gameVariables.killingSpreeDamage;
            ObjectsManager_old.instance.PlayActivationVfx(0);
        }
        //if no hit, doubles damage
        if (_objectsManager.noHitStreak)
        {
            damage *= 1.3f;
            ObjectsManager_old.instance.PlayActivationVfx(6);
        }
        enemyOld.health -= damage;
        cmShake.ShakeCamera(enemyHitShakeIntensity, .1f);
        //applies killing effects
        if (enemyOld.health <= 0)
        {
            _objectsManager.OnEnemyKill();
            GameScore_old.instance.AddScore(30);
            enemyOld.Death();
        }
        enemyOld.SliderUpdate();
        GameScore_old.instance.AddScore(10);
    }
    
    public void DealDamageToEnemy(float damageDealt, Enemy_old enemyOld, bool doShake) //when enemy takes hit
    {
        //clamps damage to an int (security)
        int damage = Mathf.CeilToInt(damageDealt);
        enemyOld.health -= damage;
        //applies killing effects
        if (enemyOld.health <= 0)
        {
            _objectsManager.OnEnemyKill();
            enemyOld.Death();
        }
        enemyOld.SliderUpdate();
    }
    
    public void ChooseGlobalVolume()
    {
        if (globalVolumes == null)
        {
            return;
        }
        //replaces lights and volumes accordingly
        switch (LevelManager.instance.currentLevel)
        {
            case 0 :
                globalVolumes[0].SetActive(true);
                globalVolumes[1].SetActive(false);
                globalVolumes[2].SetActive(false);
                lights[0].SetActive(true);
                lights[1].SetActive(false);
                lights[2].SetActive(false);
                break;
            case 1 : 
                globalVolumes[0].SetActive(false);
                globalVolumes[1].SetActive(true);
                globalVolumes[2].SetActive(false);
                lights[0].SetActive(false);
                lights[1].SetActive(true);
                lights[2].SetActive(false);
                break;
            case 2 : 
                globalVolumes[0].SetActive(false);
                globalVolumes[1].SetActive(false);
                globalVolumes[2].SetActive(true);
                lights[0].SetActive(false);
                lights[1].SetActive(false);
                lights[2].SetActive(true);
                break;
        }
        
    }
}
