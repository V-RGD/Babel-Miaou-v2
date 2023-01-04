using System.Collections.Generic;
using UnityEngine;

public class BigShooterProjectile : MonoBehaviour
{
    public int damage;
    private GameObject _player;
    private List<GameObject> _projectiles;
    public EnemyType enemyTypeData;

    private void Start()
    {
        _player = GameObject.Find("Player");
        //creates a list of projectiles for further use
        _projectiles = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject projo = Instantiate(enemyTypeData.mageProjectile);
            _projectiles.Add(projo);
            projo.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
            projo.SetActive(false);
        }
    }

    private void Update()
    {
        if ((_player.transform.position - transform.position).magnitude > 50)
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
                _projectiles[0].SetActive(true);
                _projectiles[1].SetActive(true);
                _projectiles[2].SetActive(true);
                _projectiles[3].SetActive(true);
                //shoots projectile
                Vector3 position = transform.position;
                _projectiles[0].transform.position = position + Vector3.forward * 5;
                _projectiles[1].transform.position = position + Vector3.back * 5;
                _projectiles[2].transform.position = position + Vector3.left * 5;
                _projectiles[3].transform.position = position + Vector3.right * 5;
                //gives it proper force
                _projectiles[0].GetComponent<Rigidbody>()
                    .AddForce(new Vector3(1, 0, 1) * enemyTypeData.projectileForce);
                _projectiles[1].GetComponent<Rigidbody>()
                    .AddForce(new Vector3(-1, 0, -1) * enemyTypeData.projectileForce);
                _projectiles[2].GetComponent<Rigidbody>()
                    .AddForce(new Vector3(-1, 0, 1) * enemyTypeData.projectileForce);
                _projectiles[3].GetComponent<Rigidbody>()
                    .AddForce(new Vector3(1, 0, -1) * enemyTypeData.projectileForce);
            }

            Destroy(gameObject);
        }
    }
}