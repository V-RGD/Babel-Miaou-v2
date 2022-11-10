using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class FinalBossIA : MonoBehaviour
{
    [Header("Values")]
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject lightningEyePrefab;
    public GameObject wandererPrefab;
    public LineRenderer laserL;
    public LineRenderer laserR;
    public FinalBossValues values;
    public List<string> handAttacksPool;
    public List<string> bodyAttacksPool;
    private List<string> _handAttacksQueue;
    private List<string> _bodyAttacksQueue;
    public int handAttackCount;

    public List<GameObject> lightningEyeList;
    private bool _canAttack;
    public float health;

    private float _handRespawnTimer;

    [Header("M_Laser")]
    private Vector3 _playerDir;
    private Vector3 m_laserPos;
    private Vector3 m_laserDir;
    public LaserVisuals _laserVisuals_L;
    public LaserVisuals _laserVisuals_R;
    
    [Header("Wanderer")]
    
    [Header("Claw")]
    
    [Header("Circle")]
    public float _playerDist;
    public float _circleMaxDist;
    public float _circleMinDist;
    private float _circleTimer;
    private Vector3 _circleCenter;
    private bool _circleActive;
    public GameObject circleSprite;
    public Vector3 roomCenter;
    
    [Header("EyeChain")]
    [Header("H_Laser")]
    
    public GameObject player;
    public GameManager gameManager;
    public IAStates currentState;

    private void Awake()
    {
        _laserVisuals_L.values = values;
        _laserVisuals_R.values = values;
    }

    void Start()
    {
        handAttackCount = 0;
        currentState = IAStates.EyeChain;
        _canAttack = true;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
        }
        if (Input.GetKey(KeyCode.M))
        {
        }
        if (Input.GetKey(KeyCode.R))
        {
        }
        if (Input.GetKey(KeyCode.L))
        {
        }
        BehaviourIA(currentState);
        _playerDist = (player.transform.position - roomCenter).magnitude;
    }
    
    #region M_Laser
    IEnumerator M_LaserAttack(LaserVisuals visuals)
    {
        float totalLenght = values.m_laserWarmup + 0.5f + values.m_laserLength;
        //while charging, laser is in direction of player, and color is updated depending on the current charge
        visuals.StartCoroutine(visuals.ShootLaser());
        yield return new WaitForSeconds(totalLenght);
        StartCoroutine(SwitchState(IAStates.M_Laser));
    }
    void M_Laser(LaserVisuals laser)
    {
        if (_canAttack)
        {
            _canAttack = false;
            StartCoroutine(M_LaserAttack(laser));
        }
    }
    #endregion
    
    #region Claw
    IEnumerator ClawScratchAttack()
    {
        //a warning sprite appears
        //both hand scratch the air
        yield return new WaitForSeconds(values.clawWarmup);
    }
    void ClawScratch()
    {
        if (_canAttack)
        {
            StartCoroutine(ClawScratchAttack());
        }
    }
    #endregion
    #region Wanderer
    IEnumerator SpawnWandererPrefabs()
    {
        for (int i = 0; i < values.wandererSpawnAmount; i++)
        {
            //some wanderer appears next to the player
            //take a random one for a list of available enemies
            Vector3 spawnLocation = Vector3.zero;
            GameObject enemySpawning = Instantiate(wandererPrefab, spawnLocation, Quaternion.identity);
            //enemySpawning.GetComponent<Enemy>().room = gameObject;
            enemySpawning.SetActive(true);
            enemySpawning.GetComponent<Enemy>().enabled = true;
            enemySpawning.GetComponent<EnemyDamage>().enabled = true;
            enemySpawning.GetComponent<Enemy>().startSpawning = true;
            yield return new WaitForSeconds(values.wandererSpawnInterval);
        }
    }
    void SpawnWanderers()
    {
        if (_canAttack)
        {
            StartCoroutine(SpawnWandererPrefabs());
        }
    }

    #endregion
    
    #region Circle
    IEnumerator CircleAttack()
    {
        //manages circle appearance and dissapearance
        _circleActive = true;
        _circleTimer = values.circleLength;
        circleSprite.SetActive(true);
        yield return new WaitForSeconds(values.circleLength);
        circleSprite.SetActive(false);
        _circleActive = false;
        StartCoroutine(SwitchState(IAStates.CircleAttack));
    }
    void CircleTrap()
    {
        //manages circle diminution
        _circleTimer -= Time.deltaTime;

        //circle size and detection
        float circleDist = values.circleOriginalSize * _circleTimer / values.circleLength;
        _circleMaxDist = 4*circleDist + values.circlePosInterval;
        _circleMinDist = 4*circleDist - values.circlePosInterval;

        //circle size diminishes over time
        circleSprite.transform.localScale = new Vector3(circleDist, circleDist, transform.localScale.z);

        //manages playerhitbox check
        if (_playerDist > _circleMinDist && _playerDist < _circleMaxDist && _circleActive)
        {
            gameManager.DealDamageToPlayer(values.circleDamage);
        }
        
        if (_canAttack)
        {
            _canAttack = false;
            StartCoroutine(CircleAttack());
        }
    }

    #endregion
    #region EyeChain
    IEnumerator EyeChainAttack()
    {
        yield return new WaitForSeconds(values.eyeSpawnInterval);
    }
    void EyeChain()
    {
        if (_canAttack)
        {
            StartCoroutine(EyeChainAttack());
        }
        //a bunch of eyes appear
        //where it's linked, a line renderer warning appears
        //links
    }
    #endregion
    #region H_Laser
    IEnumerator HugeLaserAttack(LineRenderer laser)
    {
        //rock warning
        //rock
        //laser warning
        //laser 
        yield return new WaitForSeconds(values.m_laserCooldown);
    }
    void HugeLaser()
    {
        if (_canAttack)
        {
            StartCoroutine(HugeLaserAttack(laserL));
        }
        //warning where the rock is supposed to fall
        //a rock appears
    }

    #endregion
    public enum IAStates
    {
        ClawScratch, 
        M_Laser,
        WandererSpawn,
        CircleAttack,
        EyeChain,
        HugeLaser
    }
    void BehaviourIA(IAStates state)
    {
        switch (state)
        {
            case IAStates.ClawScratch: 
                ClawScratch();
                break;
            case IAStates.M_Laser:
                M_Laser(_laserVisuals_L);
                M_Laser(_laserVisuals_R);
                break;
            case IAStates.WandererSpawn:
                SpawnWanderers();
                break;
            case IAStates.CircleAttack:
                CircleTrap();
                break;
            case IAStates.EyeChain:
                EyeChain();
                break;
            case IAStates.HugeLaser:
                HugeLaser();
                break;
        }
    }
    IEnumerator SwitchState(IAStates nextState)
    {
        _canAttack = false;
        yield return new WaitForSeconds(values._attackCooldown);
        currentState = nextState;
        _canAttack = true;
    }
    void CreateAttackQueue(List<string> attackPool, List<string> attackList)
    {
        //from a desired pool to a desired list
        List<string> remainingAttacks = attackPool;
        //shuffles attacks and places them in order
        for (int i = 0; i < attackPool.Count; i++)
        {
            attackList.Add(remainingAttacks[Random.Range(0, remainingAttacks.Count)]);
        }
    }
}
