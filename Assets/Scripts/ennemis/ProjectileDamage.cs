using System;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public float damage;

    private GameObject player;
    private GameManager gameManager;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
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
            gameManager.health -= damage;
            player.GetComponent<PlayerController>().invincibleCounter = player.GetComponent<PlayerController>().invincibleTime;
            Destroy(gameObject);
        }

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
