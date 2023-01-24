using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
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
    public bool canDie;
    #endregion

    #region Assignations
    [Header("Assignations")] [Space] 
    public FinalBossValues values;
    private NavMeshSurface _navMeshSurface;
    private GameObject _player;
    public RectTransform healthBar;
    //public Animator pancarteName;
    public Animator healthBarAnim;
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
    private GameObject _leftHand;
    private GameObject _rightHand;
    private VisualEffect _leftHandClawFx;
    private VisualEffect _rightHandClawFx;
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
    //public GameObject hLaserWarning;
    public VisualEffect hLaserVfx;
    public ParticleSystem hLaserChargeFx;
    public ParticleSystem hLaserSparksFx;
    public GameObject rockPrefab;
    //public GameObject rockWarning;
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
    private static readonly int SmallLaser = Animator.StringToHash("Small_Laser");
    #endregion

    #region Movement
    public Transform[] waypoints;
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
        _leftHandClawFx = clawL.transform.GetChild(0).GetComponent<VisualEffect>();
        _rightHandClawFx = clawR.transform.GetChild(0).GetComponent<VisualEffect>();

        _player = GameObject.Find("Player");
        _navMeshSurface = GameObject.Find("NavMeshSurface").GetComponent<NavMeshSurface>();
    }
    void Start()
    {
        GameManager.instance = GameManager.instance;
        healthBar = UIManager.instance.bossHealthBar;
        healthBarAnim = healthBar.transform.parent.GetComponent<Animator>();

        handAttackCount = 0;
        canActiveFirstLaser = true;
        canActiveSecondLaser = true;
        canDie = true;
        _navMeshSurface.BuildNavMesh();
        hLaserVfx.Stop();
        _circleNumber = 1;
        health = maxHealth;
        animator.CrossFade(Idle, 0, 0);
        _currentAnimatorState = Idle;
        _leftHandClawFx.Stop();
        _rightHandClawFx.Stop();
    
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
    }
    void Update()
    {
        _playerDist = (_player.transform.position - transform.position).magnitude;
        H_LaserCheck();
        LerpPosition();

        if (health <= 0 && canDie)
        {
            canDie = false;
            StartCoroutine(FinalBossDeath());
        }
        
    }
    IEnumerator M_Laser()
    {
        StartCoroutine(GoToLocation(waypoints[0].position));

        animator.CrossFade(SmallLaser, 0, 0);
        _currentAnimatorState = SmallLaser;
        float totalLenght = values.m_laserWarmup + 0.5f + values.m_laserLength;
        //while charging, laser is in direction of player, and color is updated depending on the current charge
        _laserVisualsL.StartCoroutine(_laserVisualsL.ShootLaser());
        _laserVisualsR.StartCoroutine(_laserVisualsR.ShootLaser());
        yield return new WaitForSeconds(totalLenght);
        StartCoroutine(ChooseNextAttack());
    }
    IEnumerator ClawScratch()
    {
        StartCoroutine(GoToLocation(_player.transform.position));
        yield return new WaitUntil(() => !isChangingPos);

        animator.CrossFade(Invocation, 0, 0);
        _currentAnimatorState = Invocation;
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
        StartCoroutine(GoToLocation(waypoints[0].position));
        yield return new WaitUntil(() => !isChangingPos);
        yield return new WaitForSeconds(2);
        StartCoroutine(ChooseNextAttack());
    }
    IEnumerator SpawnWanderers()
    {
        int randPos = Random.Range(4, 8);
        switch (randPos)
        {
            case 0 : StartCoroutine(GoToLocation(waypoints[1].position));
                yield return new WaitUntil(() => !isChangingPos);
                break;
            case 1 : StartCoroutine(GoToLocation(waypoints[2].position));
                yield return new WaitUntil(() => !isChangingPos);
                break;
        }

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
        StartCoroutine(GoToLocation(roomCenter.position));
        yield return new WaitUntil(() => !isChangingPos);
        yield return new WaitForSeconds(2f);
        
        float healthRatio = health / maxHealth;
        if (healthRatio is > 0.66f)
        {
            _circleNumber = 3;
        }
        if (healthRatio is < 0.66f and > 0.33f)
        {
            //if first phase
            _circleNumber = 3;
        }
        if (healthRatio < 0.33f)
        {
            //secondPhase
            _circleNumber = 3;
        }

        animator.CrossFade(Invocation, 0, 0);
        _currentAnimatorState = Invocation;
        //manages circle appearance and dissapearance
        for (int i = 0; i < _circleNumber; i++)
        {
            circleAttacks[i].gameObject.SetActive(true);
            circleAttacks[i].StartCoroutine(circleAttacks[i].CircleActivation());
            yield return new WaitForSeconds(2.5f);
        }
        yield return new WaitForSeconds(values.circleLength);
        for (int i = 0; i < _circleNumber; i++)
        {
            circleAttacks[i].gameObject.SetActive(false);
        }
        StartCoroutine(ChooseNextAttack());
    }
    IEnumerator EyeChain()
    {
        int randPos = Random.Range(1, 4);
        switch (randPos)
        {
            case 0 : StartCoroutine(GoToLocation(waypoints[1].position));
                yield return new WaitUntil(() => !isChangingPos);
                break;
            case 1 : StartCoroutine(GoToLocation(waypoints[2].position));
                yield return new WaitUntil(() => !isChangingPos);
                break;
        }

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
            Vector3 placement = startPoint + new Vector3(i * _roomSize * 2/values.eyeNumber, 2, row * _roomSize * 2/values.eyeNumber);
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
        StartCoroutine(GoToLocation(waypoints[0].position));
        yield return new WaitUntil(() => !isChangingPos);

        animator.CrossFade(Invocation, 0, 0);
        _currentAnimatorState = Invocation;
        Vector3 rockSpawnPoint = roomCenter.position + new Vector3(Random.Range(-_roomSize/2, _roomSize/2), 4, Random.Range(-_roomSize/2, _roomSize/2));
        //rock warning
        //rockWarning.SetActive(true);
        //rockWarning.transform.position = rockSpawnPoint;
        yield return new WaitForSeconds(1.2f);
        //rock
        //rockWarning.SetActive(false);
        rockPrefab.SetActive(true);
        rockPrefab.transform.position = rockSpawnPoint;
        //laser warning
        //hLaserWarning.SetActive(true);
        hLaserChargeFx.Play();
        animator.CrossFade(HugeLaser, 0, 0);
        _currentAnimatorState = HugeLaser;
        yield return new WaitForSeconds(values.hugeLaserWarmup);
        hLaserChargeFx.Stop();
        hLaserVfx.Play();
        //hLaserWarning.SetActive(false);
        yield return new WaitForSeconds(0.6f);
        //laser 
        _hLaserActive = true;
        yield return new WaitForSeconds(values.hugeLaserDuration);
        _hLaserActive = false;
        rockPrefab.SetActive(false);
        hLaserVfx.Stop();
        yield return new WaitForSeconds(values.m_laserCooldown);
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
            attackCooldown = 2;
        }
        else if (healthRatio > 0.33f)
        {
            //secondPhase
            attackCooldown = 1.5f;
        }
        else
        {
            //third phase
            attackCooldown = 1;
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
            yield break;
        }
        if (healthRatio < 0.33f && canActiveSecondLaser)
        {
            //secondPhase
            canActiveSecondLaser = false;
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
        if (_hLaserActive)
        {
            hLaserVfx.gameObject.transform.LookAt(_player.transform);
            //checks if players is in safe zone
            RaycastHit hit;
            if (Physics.Raycast(transform.position, _player.transform.position - transform.position, out hit,4000))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    //deals damage
                    GameManager.instance.DealDamageToPlayer(values.hugeLaserDamage);
                    PlayerController.instance.invincibleCounter = 2;
                }
                hLaserSparksFx.gameObject.SetActive(true);
                hLaserSparksFx.Play();
                hLaserSparksFx.transform.position = hit.point;
                Debug.Log("places fx");
            }
            else
            {
                hLaserSparksFx.transform.position = Vector3.right * 2000;
            }
            //places sparks on the ground
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
        GameManager.instance.cmShake.ShakeCamera(4, .1f);
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

    private IEnumerator FinalBossDeath()
    {
        MenuManager.instance.winScreen.gameObject.SetActive(true);
        MenuManager.instance.winScreen.CrossFade(Animator.StringToHash("Win"), 0);
        yield return new WaitForSeconds(1);
        MenuManager.instance.winScreen.gameObject.SetActive(false);
        StartCoroutine(MenuManager.instance.OpenMenu(MenuManager.instance.leaderBoardScreen.gameObject,
            MenuManager.instance.leaderBoardScreen, MenuManager.GameState.LeaderBoard));
        StartCoroutine(GameScore.instance.ShowLeaderBoards());
        //successes check
    }
    public bool isChangingPos;
    public float lerpPosTimer;
    public VfxPulling replacingPulling;
    public bool goUnderground;
    public float digSpeed;

    //fonction pour changer de position
    public IEnumerator GoToLocation(Vector3 location)
    {
        Debug.Log("switching location");
        //fais apparaitre un vfx
        isChangingPos = true;
        replacingPulling.StartCoroutine(replacingPulling.PlaceNewVfx(replacingPulling.vfxList[0], transform.position, true));
        yield return new WaitForSeconds(0.5f);
        //rentre dans le sol
        goUnderground = true;
        yield return new WaitUntil(() => lerpPosTimer >= 1);
        
        //se déplace
        transform.position = new Vector3(location.x, transform.position.y, location.z);
        replacingPulling.StartCoroutine(replacingPulling.PlaceNewVfx(replacingPulling.vfxList[0], transform.position, true));
        yield return new WaitForSeconds(0.5f);

        //ressort
        goUnderground = false;
        yield return new WaitUntil(() => lerpPosTimer <= 0);
        //unlock
        isChangingPos = false;
        Debug.Log("reached location");
    }

    public void LerpPosition()
    {
        //go from y up to y down
        transform.position = new Vector3(transform.position.x, -60 * lerpPosTimer, transform.position.z);
        if (goUnderground && lerpPosTimer < 1)
        {
            lerpPosTimer += Time.deltaTime * digSpeed;
        }
        if (!goUnderground && lerpPosTimer > 0)
        {
            lerpPosTimer -= Time.deltaTime * digSpeed;
        }
        
    }

    public IEnumerator BossApparitionSequence()
    {
        // StartCoroutine(BossApparitionSequence());
        //replaces player
        _player.transform.position =
            new Vector3(roomCenter.position.x, _player.transform.position.y, roomCenter.position.z);
        //laughts
        PlayerAttacks.instance.enabled = false;
        PlayerController.instance.enabled = false;
        animator.CrossFade(Invocation, 0, 0);
        _currentAnimatorState = Invocation;
        yield return new WaitForSeconds(1);
        //post process spoopy
        GameManager.instance.hurtVolume.CrossFade("Hurt", 0);
        //name
        //pancarteName.CrossFade("Appear", 0);
        yield return new WaitForSeconds(3);
        //not name
        healthBarAnim.CrossFade("Appear", 0);
        //active
        PlayerAttacks.instance.enabled = true;
        PlayerController.instance.enabled = true;
        yield return new WaitForSeconds(2);
        StartCoroutine(ChooseNextAttack());
    }
}
