using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public int damage;

    private GameObject player;
    private GameManager gameManager;
    private ObjectsManager _objectsManager;
    private Rigidbody _rb;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
        player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if ((player.transform.position - transform.position).magnitude > 50)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.DealDamageToPlayer(damage);
            Destroy(gameObject);
        }

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        if (other.CompareTag("PlayerAttack") && other.GetComponent<ObjectDamage>().canRepel)
        {
            _rb.velocity = -_rb.velocity;
        }
    }
}
