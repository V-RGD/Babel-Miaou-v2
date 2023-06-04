using System;
using UnityEngine;

public class KnitBall : MonoBehaviour
{
    private GameObject _player;
    private Rigidbody _rb;
    private float _bumpForce = 15;

    private float _localGravity = 10;
    private float _globalGravity = -9.81f;
    private float _frictionAmount = 1.1f;
    private float _cooldownTimer;
    private float _cooldown = 0.5f;

    public bool _isLaunched;
    public bool _isGrounded = true;

    private LayerMask _layerMask;

    private void Awake()
    {
        _player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _layerMask = LayerMask.GetMask("Ground");
    }

    void FixedUpdate ()
    {
        //custom physics
        Vector3 gravity = _globalGravity * _localGravity * Vector3.up;
        _rb.AddForce(gravity, ForceMode.Acceleration);
        
        if (!_isLaunched && _isGrounded)
        {
            _rb.velocity = new Vector3(_rb.velocity.x / _frictionAmount, _rb.velocity.y, _rb.velocity.z / _frictionAmount);
        }

        _cooldownTimer -= Time.deltaTime;
    }

    private void Update()
    {
        GroundCheck();
        
        if ((_player.transform.position - transform.position).magnitude > 80)
        {
            transform.position = new Vector3(_player.transform.position.x, 50,
                _player.transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack") && _cooldownTimer < 0)
        {
            _isLaunched = true;
            _cooldownTimer = _cooldown;
            Vector3 dir = transform.position - _player.transform.position;
            _rb.AddForce(_bumpForce * new Vector3(dir.x, 0, dir.z), ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            _isLaunched = false;
        }
    }

    void GroundCheck()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3.2f, ~_layerMask))
        {
            //Debug.DrawRay(transform.position, Vector3.down * 3.2f, Color.yellow);
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;    
        }
    }
}
