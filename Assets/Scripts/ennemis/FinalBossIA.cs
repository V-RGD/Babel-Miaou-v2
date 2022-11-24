using System;
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
    public float _maxHealth = 200;
    public float _health;
    private float _handRespawnTimer;
    private float _roomSize = 20;
    public int handAttackCount;
    #endregion

    #region Assignations
    [Header("Assignations")] [Space] 
    public FinalBossValues values;
    private GameManager _gameManager;
    private NavMeshSurface _navMeshSurface;
    private GameObject _player;
    private GameObject leftHand;
    private GameObject rightHand;
    public RectTransform healthBar;
    #endregion
    
    #region M_Laser
    [Header("M_Laser")] 
    private Vector3 _playerDir;
    private Vector3 _m_laserPos;
    private Vector3 _m_laserDir;
    private LaserVisuals _laserVisuals_L;
    private LaserVisuals _laserVisuals_R;
    #endregion

    #region Wanderer
    [Header("Wanderer")]
    public GameObject wandererPrefab;
    #endregion

    #region Claw
    [Header("Claw")]
    public GameObject clawHitbox_L;
    public GameObject clawHitbox_R;
    public GameObject clawWarning_L;
    public GameObject clawWarning_R;
    #endregion

    #region Circle
    [Header("Circle")]
    public GameObject circleSprite;
    private float _playerDist;
    private float _circleMaxDist;
    private float _circleMinDist;
    private float _circleTimer;
    private bool _circleActive;
    private Vector3 _circleCenter;
    public Vector3 roomCenter;
    #endregion

    #region EyeChain
    [Header("EyeChain")] [Header("EyeChain")]
    public GameObject eyeChainPrefab;
    [SerializeField] public List<GameObject> eyeList;
    [SerializeField] public List<GameObject> externalList;
    #endregion

    #region H_Laser
    [Header("H_Laser")]
    public GameObject _H_LaserWarning;
    public VisualEffect H_LaserVfx;
    public GameObject rockPrefab;
    public GameObject rockWarning;
    private bool H_LaserActive;
    public bool _canActiveFirstLaser;
    public bool _canActiveSecondLaser;
    #endregion
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        instance = this;
        
        _laserVisuals_L = transform.GetChild(0).GetComponent<LaserVisuals>();
        _laserVisuals_R = transform.GetChild(1).GetComponent<LaserVisuals>();
        _laserVisuals_L.values = values;
        _laserVisuals_R.values = values;
        _player = GameObject.Find("Player");
        _navMeshSurface = GameObject.Find("NavMeshSurface").GetComponent<NavMeshSurface>();
    }
    void Start()
    {
        _gameManager = GameManager.instance;

        handAttackCount = 0;
        _canActiveFirstLaser = true;
        _canActiveSecondLaser = true;
        _navMeshSurface.BuildNavMesh();
        H_LaserVfx.Stop();

        for (int i = 0; i < values.eyeNumber; i++)
        {
            GameObject eye = Instantiate(eyeChainPrefab);
            eye.GetComponent<EyeChain>().ia = this;
            eye.SetActive(false);
            eyeList.Add(eye);
        }
        for (int i = 0; i < externalList.Count; i++)
        {
            externalList[i].GetComponent<EyeChain>().ia = this;
        }

        StartCoroutine(ChooseNextAttack());
    }
    void Update()
    {
        _playerDist = (_player.transform.position - transform.position).magnitude;
        Debug.Log(_playerDist);
        H_LaserCheck();
        CircleManagement();
    }
    IEnumerator M_Laser()
    {
        float totalLenght = values.m_laserWarmup + 0.5f + values.m_laserLength;
        //while charging, laser is in direction of player, and color is updated depending on the current charge
        _laserVisuals_L.StartCoroutine(_laserVisuals_L.ShootLaser());
        _laserVisuals_R.StartCoroutine(_laserVisuals_R.ShootLaser());
        yield return new WaitForSeconds(totalLenght);
        StartCoroutine(ChooseNextAttack());
    }
    IEnumerator ClawScratch()
    {
        //a warning sprite appears
        clawWarning_L.SetActive(true);
        clawWarning_R.SetActive(true);
        //both hand scratch the air
        yield return new WaitForSeconds(values.clawWarmup);
        clawWarning_L.SetActive(false);
        clawWarning_R.SetActive(false);
        clawHitbox_L.SetActive(true);
        clawHitbox_R.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        clawHitbox_L.SetActive(false);
        clawHitbox_R.SetActive(false);
        yield return new WaitForSeconds(2);
        StartCoroutine(ChooseNextAttack());
    }
    IEnumerator SpawnWanderers()
    {
        for (int i = 0; i < values.wandererSpawnAmount; i++)
        {
            //some wanderer appears next to the player
            Vector3 spawnLocation = roomCenter + new Vector3(Random.Range(-_roomSize, _roomSize), 0, Random.Range(-_roomSize, _roomSize)) + Vector3.up * 0.8f;
            GameObject enemySpawning = Instantiate(wandererPrefab, spawnLocation, Quaternion.identity);
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
        //manages circle appearance and dissapearance
        _circleActive = true;
        _circleTimer = values.circleLength;
        circleSprite.SetActive(true);
        yield return new WaitForSeconds(values.circleLength);
        circleSprite.SetActive(false);
        _circleActive = false;
        StartCoroutine(ChooseNextAttack());
    }
    IEnumerator EyeChain()
    {
        //-------------------------------------------------------------------------place les yeux
        
        List<int> eyeChainRows = new List<int>(values.eyeNumber);
        //creates a grid with x separations on each side, scaled to the terrain
        Vector3 startPoint = roomCenter - new Vector3(_roomSize / 2, 0, _roomSize / 2);
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
            Vector3 placement = startPoint + new Vector3(i * _roomSize/values.eyeNumber, 1, row * _roomSize/values.eyeNumber);
            eyeList[i].transform.position = placement;
            eyeList[i].SetActive(true);
            eyeChainRows.Add(row);
            yield return new WaitForSeconds(0.1f);
        }
        eyeList[4].GetComponent<EyeChain>().isbase = true;
        //----attend un peu
        foreach (var eye in eyeList)
        {
            StartCoroutine(eye.GetComponent<EyeChain>().CheckConnection());
        }
        foreach (var eye in externalList)
        {
            StartCoroutine(eye.GetComponent<EyeChain>().CheckConnection());
        }
        yield return new WaitForSeconds(4);

        StartCoroutine(ChooseNextAttack());    
    }
    IEnumerator H_Laser()
    {
        Vector3 rockSpawnPoint = roomCenter + new Vector3(Random.Range(-_roomSize, _roomSize), 4, Random.Range(-_roomSize, _roomSize));
        //rock warning
        rockWarning.SetActive(true);
        rockWarning.transform.position = rockSpawnPoint;
        yield return new WaitForSeconds(1.2f);
        //rock
        rockWarning.SetActive(false);
        rockPrefab.SetActive(true);
        rockPrefab.transform.position = rockSpawnPoint;
        //laser warning
        _H_LaserWarning.SetActive(true);
        yield return new WaitForSeconds(2);
        _H_LaserWarning.SetActive(false);
        //laser 
        H_LaserActive = true;
        H_LaserVfx.gameObject.SetActive(true);
        H_LaserVfx.Play();
        yield return new WaitForSeconds(2);
        H_LaserActive = false;
        rockPrefab.SetActive(false);
        yield return new WaitForSeconds(values.m_laserCooldown);
        H_LaserVfx.gameObject.SetActive(false);
        StartCoroutine(ChooseNextAttack());    
    }
    IEnumerator ChooseNextAttack()
    {
        float attackCooldown;
        //determines attack cooldown
        float healthRatio = _health / _maxHealth;
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
        if (healthRatio is < 0.66f and > 0.33f && _canActiveFirstLaser)
        {
            //if first phase
            _canActiveFirstLaser = false;
            StartCoroutine(H_Laser());
            yield break;
        }
        if (healthRatio < 0.33f && _canActiveSecondLaser)
        {
            //secondPhase
            _canActiveSecondLaser = false;
            StartCoroutine(H_Laser());
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
        if (H_LaserActive)
        {
            //checks if players is in safe zone
            RaycastHit hit;
            if (Physics.Raycast(transform.position, _player.transform.position - transform.position, out hit,4000))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    //deals damage
                    _gameManager.DealDamageToPlayer(values.hugeLaserDamage);
                }
            } 
        }
    }
    void CircleManagement()
    {
        if (_circleActive)
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
        }
    }
    void TakeDamage(float damageDealt) //when enemy takes hit
    {
        //clamps damage to an int (security)
        int damage = Mathf.CeilToInt(damageDealt);
        //applies damage
        _health -= damage;
        _gameManager._cmShake.ShakeCamera(4, .1f);
        healthBar.sizeDelta = new Vector2(1323.4f * _health / _maxHealth, 12.95f);
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
