using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class FinalBossIA : MonoBehaviour
{
    public static FinalBossIA instance;
    
    #region Global Values

    [Header("Values")] 
    public float maxHealth = 200;
    public float health;
    private float _handRespawnTimer;
    private float _roomSize = 20;
    private int handAttackCount;
    #endregion

    #region Assignations
    [Header("Assignations")] [Space] 
    public FinalBossValues values;
    private GameManager _gameManager;
    private NavMeshSurface _navMeshSurface;
    private GameObject _player;
    private GameObject _leftHand;
    private GameObject _rightHand;
    public RectTransform healthBar;
    #endregion
    
    #region M_Laser
    [Header("M_Laser")] 
    private Vector3 _playerDir;
    private Vector3 _mLaserPos;
    private Vector3 _mLaserDir;
    private LaserVisuals _laserVisualsL;
    private LaserVisuals _laserVisualsR;
    #endregion

    #region Claw

    [Header("Claw")] public Transform attackAnchor;
    public GameObject clawL;
    public GameObject clawR;
    #endregion

    #region Circle

    [Header("Circle")] 
    public List<CircleAttack> circleAttacks;
    private float _playerDist;
    public Transform roomCenter;
    private int _circleNumber = 1;
    #endregion

    #region EyeChain
    [Header("EyeChain")] [Header("EyeChain")]
    public GameObject eyeChainPrefab;
    [HideInInspector] public List<GameObject> eyeList;
    [HideInInspector] public List<EyeChain> eyeChains;
    public List<EyeChain> eyeChainsExternal;
    [SerializeField] private List<GameObject> externalList;
    #endregion

    #region H_Laser
    [Header("H_Laser")]
    public GameObject hLaserWarning;
    public VisualEffect hLaserVfx;
    public GameObject rockPrefab;
    public GameObject rockWarning;
    private bool _hLaserActive;
    private bool canActiveFirstLaser;
    private bool canActiveSecondLaser;
    #endregion

    #region Animator
    private int _currentAnimatorState;
    public Animator animator;
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Invocation = Animator.StringToHash("Invocation");
    private static readonly int HugeLaser = Animator.StringToHash("Huge_Laser");
    #endregion

    #region Movement

    

    #endregion
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        instance = this;

        _laserVisualsL = transform.GetChild(0).GetComponent<LaserVisuals>();
        _laserVisualsR = transform.GetChild(1).GetComponent<LaserVisuals>();
        _laserVisualsL.values = values;
        _laserVisualsR.values = values;
        _player = GameObject.Find("Player");
        _navMeshSurface = GameObject.Find("NavMeshSurface").GetComponent<NavMeshSurface>();
    }
    void Start()
    {
        _gameManager = GameManager.instance;
        healthBar.transform.parent.gameObject.SetActive(true);

        handAttackCount = 0;
        canActiveFirstLaser = true;
        canActiveSecondLaser = true;
        _navMeshSurface.BuildNavMesh();
        hLaserVfx.Stop();
        _circleNumber = 1;
        health = maxHealth;
        animator.CrossFade(Idle, 0, 0);
        _currentAnimatorState = Idle;
    
        for (int i = 0; i < values.eyeNumber; i++)
        {
            GameObject eye = Instantiate(eyeChainPrefab);
            eye.GetComponent<EyeChain>().ia = this;
            eye.SetActive(false);
            eyeList.Add(eye);
            eyeChains.Add(eye.GetComponent<EyeChain>());
        }
        for (int i = 0; i < externalList.Count; i++)
        {
            externalList[i].GetComponent<EyeChain>().ia = this;
            eyeChainsExternal.Add(externalList[i].GetComponent<EyeChain>());
            eyeChainsExternal[i].ia = this;
        }

        StartCoroutine(ChooseNextAttack());
    }
    void Update()
    {
        _playerDist = (_player.transform.position - transform.position).magnitude;
        H_LaserCheck();
    }
    IEnumerator M_Laser()
    {
        float totalLenght = values.m_laserWarmup + 0.5f + values.m_laserLength;
        //while charging, laser is in direction of player, and color is updated depending on the current charge
        _laserVisualsL.StartCoroutine(_laserVisualsL.ShootLaser());
        _laserVisualsR.StartCoroutine(_laserVisualsR.ShootLaser());
        yield return new WaitForSeconds(totalLenght);
        StartCoroutine(ChooseNextAttack());
    }
    IEnumerator ClawScratch()
    {
        Vector3 playerPos = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
        attackAnchor.LookAt(playerPos);
        //a warning sprite appears
        clawL.transform.GetChild(1).gameObject.SetActive(true);
        clawR.transform.GetChild(1).gameObject.SetActive(true);
        //both hand scratch the air
        yield return new WaitForSeconds(values.clawWarmup);
        clawL.transform.GetChild(1).gameObject.SetActive(false);
        clawR.transform.GetChild(1).gameObject.SetActive(false);
        //left claw
        clawL.GetComponent<BoxCollider>().enabled = true;
        clawL.transform.GetChild(0).GetComponent<VisualEffect>().Play();
        yield return new WaitForSeconds(0.5f);
        //right claw
        clawL.GetComponent<BoxCollider>().enabled = false;
        clawR.GetComponent<BoxCollider>().enabled = true;
        clawR.transform.GetChild(0).GetComponent<VisualEffect>().Play();
        yield return new WaitForSeconds(0.5f);
        clawR.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(2);
        StartCoroutine(ChooseNextAttack());
    }
    IEnumerator SpawnWanderers()
    {
        animator.CrossFade(Invocation, 0, 0);
        _currentAnimatorState = Invocation;
        for (int i = 0; i < values.wandererSpawnAmount; i++)
        {
            //some wanderer appears next to the player
            Vector3 spawnLocation = roomCenter.position + new Vector3(Random.Range(-_roomSize/2, _roomSize/2), 0, Random.Range(-_roomSize/2, _roomSize/2)) + Vector3.up * 0.8f;
            GameObject enemySpawning = Instantiate(LevelManager.instance.basicEnemies[0], spawnLocation, Quaternion.identity);
            enemySpawning.SetActive(true);
            Enemy enemy = enemySpawning.GetComponent<Enemy>();
            enemy.room = gameObject;
            enemy.enabled = true;
            enemy.StartCoroutine(enemy.EnemyApparition());
            yield return new WaitForSeconds(values.wandererSpawnInterval);
        }
        yield return new WaitForSeconds(2);
        StartCoroutine(ChooseNextAttack());
    }
    IEnumerator CircleTrap()
    {
        animator.CrossFade(Invocation, 0, 0);
        _currentAnimatorState = Invocation;
        //manages circle appearance and dissapearance
        for (int i = 0; i < _circleNumber; i++)
        {
            circleAttacks[i].StartCoroutine(circleAttacks[i].CircleActivation());
            yield return new WaitForSeconds(2.5f);
        }
        yield return new WaitForSeconds(values.circleLength);
        StartCoroutine(ChooseNextAttack());
    }
    IEnumerator EyeChain()
    {
        animator.CrossFade(Invocation, 0, 0);
        _currentAnimatorState = Invocation;
        //-------------------------------------------------------------------------place les yeux
        
        List<int> eyeChainRows = new List<int>(values.eyeNumber);
        //creates a grid with x separations on each side, scaled to the terrain
        Vector3 startPoint = roomCenter.position - new Vector3(_roomSize / 2, 0, _roomSize / 2);
        //creates a list of 10possible placements
        List<int> possibleRows = new List<int>(0);
        for (int i = 0; i < values.eyeNumber; i++)
        {
            possibleRows.Add(i);
        }
        
        //-------------------------------------------------------------------distribue les placements en faisant gaffe a ce qu'il y ait pas 2 yeux sur la même colonne
        for (int i = 0; i < values.eyeNumber; i++)
        {
            //place l'oeil sur la ligne i, avec la position de la colonne aléatoire
            int row = possibleRows[Random.Range(0, possibleRows.Count)];
            possibleRows.Remove(row);
            Vector3 placement = startPoint + new Vector3(i * _roomSize * 2/values.eyeNumber, 1, row * _roomSize * 2/values.eyeNumber);
            eyeList[i].transform.position = placement;
            eyeList[i].SetActive(true);
            eyeChainRows.Add(row);
            yield return new WaitForSeconds(0.1f);
        }

        eyeChains[4].isbase = true;
        //----attend un peu
        foreach (var eye in eyeChains)
        {
            StartCoroutine(eye.CheckConnection());
        }
        foreach (var eye in eyeChainsExternal)
        {
            StartCoroutine(eye.CheckConnection());
        }
        yield return new WaitForSeconds(4);

        StartCoroutine(ChooseNextAttack());    
    }
    IEnumerator H_Laser()
    {
        animator.CrossFade(HugeLaser, 0, 0);
        _currentAnimatorState = HugeLaser;
        Vector3 rockSpawnPoint = roomCenter.position + new Vector3(Random.Range(-_roomSize/2, _roomSize/2), 4, Random.Range(-_roomSize/2, _roomSize/2));
        //rock warning
        rockWarning.SetActive(true);
        rockWarning.transform.position = rockSpawnPoint;
        yield return new WaitForSeconds(1.2f);
        //rock
        rockWarning.SetActive(false);
        rockPrefab.SetActive(true);
        rockPrefab.transform.position = rockSpawnPoint;
        //laser warning
        hLaserWarning.SetActive(true);
        yield return new WaitForSeconds(4);
        hLaserWarning.SetActive(false);
        //laser 
        _hLaserActive = true;
        hLaserVfx.gameObject.SetActive(true);
        hLaserVfx.Play();
        yield return new WaitForSeconds(2);
        _hLaserActive = false;
        rockPrefab.SetActive(false);
        yield return new WaitForSeconds(values.m_laserCooldown);
        hLaserVfx.gameObject.SetActive(false);
        StartCoroutine(ChooseNextAttack());    
    }
    IEnumerator ChooseNextAttack()
    {
        animator.CrossFade(Idle, 0, 0);
        _currentAnimatorState = Idle;
        float attackCooldown;
        //determines attack cooldown
        float healthRatio = health / maxHealth;
        if (healthRatio > 0.66f)
        {
            //if first phase
            attackCooldown = 3;
        }
        else if (healthRatio > 0.33f)
        {
            //secondPhase
            attackCooldown = 2.5f;
        }
        else
        {
            //third phase
            attackCooldown = 2;
        }

        yield return new WaitForSeconds(attackCooldown);

        float meleeRange = 25;
        float handsAvailable = 2;
        
        //---------------------checks before if it must shoot the Huge Laser
        if (healthRatio is < 0.66f and > 0.33f && canActiveFirstLaser)
        {
            //if first phase
            canActiveFirstLaser = false;
            StartCoroutine(H_Laser());
            _circleNumber = 2;
            yield break;
        }
        if (healthRatio < 0.33f && canActiveSecondLaser)
        {
            //secondPhase
            canActiveSecondLaser = false;
            StartCoroutine(H_Laser());
            _circleNumber = 3;
            yield break;
        }
        //if attack counter inferior to the required amount to play body attacks, and at least one hand is available, plays hand attack
        if (handAttackCount < 4 && handsAvailable != 0)
        {
            //checks distance
            if (_playerDist < meleeRange)
            {
                //plays either spawn or claw
                int randomizeAttack = Random.Range(0, 2);
                switch (randomizeAttack)
                {
                    case 0:
                        StartCoroutine(ClawScratch());
                        break;
                    case 1:
                        StartCoroutine(SpawnWanderers());
                        break;
                }
            }
            else
            {
                //spams lasers
                StartCoroutine(M_Laser());
            }
            //adds one to the counter
            handAttackCount++;
        }
        else
        {
            //resets counter
            handAttackCount = 0;
            //plays either spawn or claw
            int randomizeAttack = Random.Range(0, 2);
            switch (randomizeAttack)
            {
                case 0:
                    StartCoroutine(CircleTrap());
                    break;
                case 1:
                    StartCoroutine(EyeChain());
                    break;
            }
        }
    }
    void H_LaserCheck()
    {
        if (_hLaserActive)
        {
            //checks if players is in safe zone
            RaycastHit hit;
            if (Physics.Raycast(transform.position, _player.transform.position - transform.position, out hit,4000))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    //deals damage
                    _gameManager.DealDamageToPlayer(values.hugeLaserDamage);
                    PlayerController.instance.invincibleCounter = 2;
                }
            } 
        }
    }
    void TakeDamage(float damageDealt) //when enemy takes hit
    {
        var position = transform.position;
        EnemyVfx.instance.hitFx.StartCoroutine(EnemyVfx.instance.hitFx.PlaceNewVfx(EnemyVfx.instance.hitFx.particleList[0], position, true));
        EnemyVfx.instance.hitFx.StartCoroutine(EnemyVfx.instance.hitFx.PlaceNewVfx(EnemyVfx.instance.hitFx.particleList[1], position, true));
        EnemyVfx.instance.hitFx.StartCoroutine(EnemyVfx.instance.hitFx.PlaceNewVfx(EnemyVfx.instance.hitFx.particleList[2], position, true));
        
        //clamps damage to an int (security)
        int damage = Mathf.CeilToInt(damageDealt);
        //applies damage
        health -= damage;
        _gameManager.cmShake.ShakeCamera(4, .1f);
        healthBar.sizeDelta = new Vector2(1323.4f * health / maxHealth, 12.95f);
    }
    private void OnTriggerEnter(Collider other)
    {
        //if player hit
        if (other.CompareTag("PlayerAttack"))
        {
            TakeDamage(other.GetComponent<ObjectDamage>().damage);
        }
    }
}
