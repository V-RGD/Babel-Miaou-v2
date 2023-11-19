using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public int damage;

    private GameObject player;
    private Rigidbody _rb;
    private void Start()
    {
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
            GameManager_old.instance.DealDamageToPlayer(damage);
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
