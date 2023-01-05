using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public GameObject[] items;
    private GameVariables _gameVariables;
    private Room _room;
    [HideInInspector] public CinemachineShake cmShake;
    public List<Transform> eyesInGame;
    private EnemyHitFx _enemyHitFx;

    public int money;
    public int maxHealth = 3;
    public int health;
    public int healthBonus;
    public float enemyHitShakeIntensity = 3;

    public int playerRoom;
    public bool isDead;
    private bool _isFreezed;
    public int currentLevel;

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
        ObjectsManager.instance = ObjectsManager.instance;
        _gameVariables = ObjectsManager.instance.gameVariables;
        PlayerController.instance = PlayerController.instance;
        UIManager.instance = UIManager.instance;
        PlayerAttacks.instance = PlayerAttacks.instance;
        health = maxHealth;
        UIManager.instance.HealthBar(health);
    }

    IEnumerator FreezeFrame(float length)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(length);
        Time.timeScale = 0;
        _isFreezed = false;
    }
    
    public void DealDamageToPlayer(float damageDealt) //when player takes hit
    {
        if (PlayerController.instance.invincibleCounter <= 0 && ObjectsManager.instance.sacredCrossTimer <= 0 && !PlayerController.instance.isDashing)
        {
            //clamps damage to an int (security)
            int damage = Mathf.CeilToInt(damageDealt);
            //applies effects --- including damage taken
            ObjectsManager.instance.OnPlayerHit(damage);
            //player is invincible for a time
            PlayerController.instance.invincibleCounter = PlayerController.instance.invincibleTime;
            //sets health bar
            UIManager.instance.HealthBar(health);
            UIManager.instance.HurtPanels();
            PlayerController.instance.invincibleCounter = 1;
            cmShake.ShakeCamera(7, .1f);
            
            //ded
            if (health <= 0)
            {
                if (ObjectsManager.instance.catLuck)
                {
                    //destroys catnip
                    for (int i = 0; i < 3; i++)
                    {
                        if (ObjectsManager.instance.itemObjectsInventory[i] == 4)
                        {
                            //destroys item
                            ObjectsManager.instance.OnObjectUnEquip(4);
                            //adds new one
                            ObjectsManager.instance.itemObjectsInventory[i] = 999;
                            UIManager.instance.UpdateHUDIcons();
                        }
                    }
                    //adds health
                    health = Mathf.CeilToInt(_gameVariables.catLuckResHp);
                    UIManager.instance.HealthBar(health);
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
        //enemy.hitFX.gameObject.SetActive(true);
        //enemy.hitFX.Play();
        _enemyHitFx.StartCoroutine(_enemyHitFx.PlaceNewVfx(enemy.transform.position));
        //clamps damage to an int (security)
        float damage = damageDealt;
        //applies damage
        if (ObjectsManager.instance.killingSpreeTimer > 0)
        {
            damage++;
        }
        //if no hit, doubles damage
        if (ObjectsManager.instance.noHitStreak)
        {
            damage *= 2;
        }
        enemy.health -= damage;
        //Debug.Log(damage);
        cmShake.ShakeCamera(enemyHitShakeIntensity, .1f);
        //applies killing effects
        if (enemy.health <= 0)
        {
            ObjectsManager.instance.OnEnemyKill();
            GameScore.instance.AddScore(30);
            enemy.Death();
        }
        enemy.SliderUpdate();
        GameScore.instance.AddScore(10);
    }
    
    public void DealDamageToEnemy(float damageDealt, Enemy enemy, bool doShake) //when enemy takes hit
    {
        //clamps damage to an int (security)
        //int damage = Mathf.CeilToInt(damageDealt);
        enemy.health -= damageDealt;
        //applies killing effects
        if (enemy.health <= 0)
        {
            ObjectsManager.instance.OnEnemyKill();
            enemy.Death();
        }
        enemy.SliderUpdate();
    }
}
