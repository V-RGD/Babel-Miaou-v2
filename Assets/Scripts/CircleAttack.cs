using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class CircleAttack : MonoBehaviour
{
    public bool isActive;
    private float _sizeTimer;
    private float _maxDist;
    private float _minDist;
    private GameObject _player;
    private GameManager _gameManager;
    private float _playerDist;
    public float damage;
    public float speed;
    public float detectionOffset = 0.05f;
    private SpriteRenderer _spriteRenderer;
    public VisualEffect fx;
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _player = GameObject.Find("Player");
        _gameManager = GameManager.instance;
        fx = transform.GetChild(0).GetComponent<VisualEffect>();
    }

    private void Update()
    {
        if (isActive)
        {
            _playerDist = (_player.transform.position - transform.position).magnitude;
            PlayerDetection();
        }
    }

    void PlayerDetection()
    {
        //manages circle diminution
        _sizeTimer += Time.deltaTime;

        //circle size and detection
        float circleDist = _sizeTimer * speed;
        _maxDist = circleDist/2 + detectionOffset;
        _minDist = circleDist/2 - detectionOffset;
        transform.localScale = new Vector3(circleDist, circleDist, circleDist);

        //manages player hitbox check
        if (_playerDist > _minDist && _playerDist < _maxDist && PlayerController.instance.invincibleCounter <= 0)
        {
            _gameManager.DealDamageToPlayer(damage);
        }
    }

    public IEnumerator CircleActivation()
    {
        isActive = true;
        _spriteRenderer.enabled = true;
        //sets detector at 0, circle at 0 size
        _sizeTimer = 0;
        fx.gameObject.SetActive(true);
        fx.Play();
        //waits until circle is out of screen
        yield return new WaitUntil((() => _sizeTimer * speed > 100));
        //disables
        fx.gameObject.SetActive(false);
        isActive = false;
        _spriteRenderer.enabled = false;
    }
}
