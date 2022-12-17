using UnityEngine;

public class EnemyWarnings : MonoBehaviour
{
    private SpriteRenderer _enemySprite;
    private GameObject _warningUi;
    [SerializeField]private int type;
    private UIManager _uiManager;
    private float _offset;
    private float _xCamCoord = 900;
    private float _yCamCoord = 477;
    private Transform player;
    private Enemy enemy;
    private void Start()
    {
        enemy = GetComponent<Enemy>();
        _uiManager = UIManager.instance;
        _enemySprite = enemy.sprite.GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player").transform;
        
        //creates new warning
        GameObject warning = Instantiate(_uiManager.enemyWarnings[type], _uiManager.enemyWarningGroup);
        _warningUi = warning;
    }

    private void Update()
    {
        if (enemy.isActive)
        {
            CheckEnemyOnScreen();
        }
        else
        {
            _warningUi.SetActive(false);
        }
    }

    void CheckEnemyOnScreen()
    {
        //if enemy not on screen
        if (!_enemySprite.isVisible)
        {
            //display warning
            _warningUi.SetActive(true);
            //calculates player direction to put the warning in the right direction
            Vector3 enemyDir = (transform.position - player.position).normalized;
            _warningUi.transform.localPosition = new Vector3(_xCamCoord * enemyDir.x, _yCamCoord * enemyDir.z, 0);
        }
        else
        {
            _warningUi.SetActive(false);
        }
    }
}
