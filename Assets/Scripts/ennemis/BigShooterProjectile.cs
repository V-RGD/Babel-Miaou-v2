using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigShooterProjectile : MonoBehaviour
{
    public int damage;

    private GameObject player;
    private GameManager gameManager;
    private ObjectsManager _objectsManager;
    private Rigidbody _rb;
    private List<GameObject> _projeciles;
    public EnemyType enemyTypeData;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
        player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();

        //creates a list of projectiles for further use
        _projeciles = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject projo = Instantiate(enemyTypeData.mageProjectile);
            _projeciles.Add(projo);
            projo.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
            projo.SetActive(false);
        }
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
        //if coming from a big shooter, when hitting a surface it will split in multiples smaller ones

        if (other.CompareTag("Wall") || other.CompareTag("Player"))
        {
            for (int i = 0; i < 4; i++)
            {
                //actives projo
                _projeciles[0].SetActive(true);
                _projeciles[1].SetActive(true);
                _projeciles[2].SetActive(true);
                _projeciles[3].SetActive(true);
                //shoots projectile
                _projeciles[0].transform.position = transform.position + Vector3.forward * 5;
                _projeciles[1].transform.position = transform.position + Vector3.back * 5;
                _projeciles[2].transform.position = transform.position + Vector3.left * 5;
                _projeciles[3].transform.position = transform.position + Vector3.right * 5;
                //gives it proper force
                _projeciles[0].GetComponent<Rigidbody>()
                    .AddForce(new Vector3(1, 0, 1) * enemyTypeData.projectileForce);
                _projeciles[1].GetComponent<Rigidbody>()
                    .AddForce(new Vector3(-1, 0, -1) * enemyTypeData.projectileForce);
                _projeciles[2].GetComponent<Rigidbody>()
                    .AddForce(new Vector3(-1, 0, 1) * enemyTypeData.projectileForce);
                _projeciles[3].GetComponent<Rigidbody>()
                    .AddForce(new Vector3(1, 0, -1) * enemyTypeData.projectileForce);
            }

            Destroy(gameObject);
        }
    }
}