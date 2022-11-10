using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class FinalBossIA : MonoBehaviour
{
    #region Global Values
    [Header("Values")]
    //public LineRenderer HLaser_LineRenderer;
    public FinalBossValues values;
    public List<string> handAttacksPool;
    public List<string> bodyAttacksPool;
    private List<string> _handAttacksQueue;
    private List<string> _bodyAttacksQueue;
    public int handAttackCount;
    #endregion
    
    #region Assignations
    [Header("Assignations")]
    [Space]
    private GameObject _player;
    private GameManager _gameManager;
    private GameObject leftHand;
    private GameObject rightHand;
    public GameObject lightningEyePrefab;
    public GameObject wandererPrefab;
    public GameObject rockPrefab;
    public GameObject HLaser;
    public GameObject HLaser_ScopeTo;
    [Space]
    public GameObject _leftClawWarning;
    public GameObject _rightClawWarning;
    public GameObject _laserWarning;
    public GameObject _rockWarning;
    #endregion
    
    private List<GameObject> _lightningEyeList;
    private bool _canAttack;
    public float health;
    private float _handRespawnTimer;

    #region M_Laser
    [Header("M_Laser")]
    private Vector3 _playerDir;
    private Vector3 m_laserPos;
    private Vector3 m_laserDir;
    private LaserVisuals _laserVisuals_L;
    private LaserVisuals _laserVisuals_R;
    private GameObject _laserHitbox;
    #endregion

    #region Wanderer
    [Header("Wanderer")]
    #endregion

    #region Claw
    [Header("Claw")]
    #endregion

    #region Circle
    [Header("Circle")]
    public float _playerDist;
    public float _circleMaxDist;
    public float _circleMinDist;
    private float _circleTimer;
    private Vector3 _circleCenter;
    private bool _circleActive;
    public GameObject circleSprite;
    public Vector3 roomCenter;
    #endregion

    #region EyeChain
    [Header("EyeChain")] 
    #endregion

    #region HLaser
    [Header("HLaser")]
    public float HLaserRotation;
    public float HLaserTimer;
    public bool HLaserActive;
    private bool _isHLaserCharging;
    private float _HLaserTimer;
    private Material _HlaserMat;
    private Vector3 _HLaserDir;
    #endregion

    public IAStates currentState;
    private void Awake()
    {
        _laserVisuals_L = transform.GetChild(0).GetComponent<LaserVisuals>();
        _laserVisuals_R = transform.GetChild(1).GetComponent<LaserVisuals>();
        _laserVisuals_L.values = values;
        _laserVisuals_R.values = values;
        //HLaser_LineRenderer = GetComponent<LineRenderer>();
        //_HlaserMat = HLaser_LineRenderer.material;
        _player = GameObject.Find("Player");
        _laserHitbox = HLaser.transform.GetChild(0).gameObject;
        _laserHitbox.GetComponent<HugeLaserDamage>().damage = values.hugeLaserDamage;
    }
    void Start()
    {
        handAttackCount = 0;
        currentState = IAStates.HugeLaser;
        _canAttack = true;
    }
    void Update()
    {
        BehaviourIA(currentState);
        _playerDist = (_player.transform.position - roomCenter).magnitude;
        VisualsUpdate();
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
            _gameManager.DealDamageToPlayer(values.circleDamage);
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
    #region HLaser
    IEnumerator HugeLaserAttack()
    {
        float roomSize = 20;
        Vector3 rockSpawnPoint = roomCenter + new Vector3(Random.Range(-roomSize, roomSize), 0, Random.Range(-roomSize, roomSize));
        //rock warning
        _rockWarning.SetActive(true);
        _rockWarning.transform.position = rockSpawnPoint;
        yield return new WaitForSeconds(1.2f);
        //rock
        _rockWarning.SetActive(false);
        rockPrefab.SetActive(true);
        rockPrefab.transform.position = rockSpawnPoint + Vector3.up * 10;
        //laser warning
        _laserWarning.SetActive(true);
        yield return new WaitForSeconds(2);
        _laserWarning.SetActive(false);
        //laser 
        _laserHitbox.SetActive(true);
        HLaserTimer = 0;
        HLaserActive = true;
        yield return new WaitForSeconds(1);
        HLaserActive = false;
        _laserHitbox.SetActive(false);
        rockPrefab.SetActive(false);
        yield return new WaitForSeconds(values.m_laserCooldown);
        StartCoroutine(SwitchState(IAStates.HugeLaser));
    }
    void HugeLaser()
    {
        if (_canAttack)
        {
            _canAttack = false;
            StartCoroutine(HugeLaserAttack());
        }
    }
    void VisualsUpdate()
    {
        if (HLaserActive)
        {
            HLaserTimer += Time.deltaTime;
            HLaser.transform.rotation = Quaternion.Euler(0, -70 * (1 + -2 * HLaserTimer), 0);
            //updates position
            _HLaserDir = (HLaser_ScopeTo.transform.position - transform.position).normalized;
            //_HlaserMat.color = Color.magenta;
                
            //updates laser position
            Vector3 hitPoint;
            RaycastHit hit;
            
            //check if a wall is in between laser
            if (Physics.Raycast(transform.position, _playerDir, out hit, 1000, values.wallLayerMask))
            {
                hitPoint = hit.point;
            }
            else
            {
                hitPoint = transform.position + _HLaserDir.normalized * 200;
            }
            
            //HLaser_LineRenderer.SetPosition(0, transform.position);
            //HLaser_LineRenderer.SetPosition(1, hitPoint);
            _laserHitbox.transform.localScale = new Vector3(5, 15, (hitPoint - transform.position).magnitude);
            //HLaser.transform.position = transform.position + (hitPoint - transform.position / 2);

            //check if player touches laser
            if (Physics.Raycast(transform.position, _HLaserDir, 1000, values.playerLayerMask))
            {
                Debug.Log("hit player");
                //deals damage
                _gameManager.DealDamageToPlayer(values.m_laserDamage);
                //can touch laser twice
            }
        }
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
