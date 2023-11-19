using System;
using UnityEngine;

public class AspireurProjectile : MonoBehaviour
{
    public int damage;

    private GameObject player;
    private GameManager_old gameManager;
    private Rigidbody _rb;
    private LayerMask wallLayerMask;
    private LayerMask enemyLayerMask;
    public Vector3 wallCheckDir;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager_old>();
        player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();
        wallLayerMask = LayerMask.GetMask("Wall");
        enemyLayerMask = LayerMask.GetMask("Pull");
    }

    private void FixedUpdate()
    {
        wallCheckDir = _rb.velocity.normalized;
        if (Physics.Raycast(transform.position, wallCheckDir, 4, wallLayerMask))
        {
            _rb.velocity = Vector3.zero;
        }
        wallCheckDir = _rb.velocity.normalized;
        if (Physics.Raycast(transform.position, wallCheckDir, 4, enemyLayerMask))
        {
            _rb.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack") && other.GetComponent<ObjectDamage>().canRepel)
        {
            _rb.velocity = (transform.position - player.transform.position).normalized * _rb.velocity.magnitude;
        }
        if (other.CompareTag("Player"))
        {
            gameManager.health -= damage;
            player.GetComponent<PlayerController__old>().invincibleCounter = player.GetComponent<PlayerController__old>().invincibleTime;
        }
    }
}
