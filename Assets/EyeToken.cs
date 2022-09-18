using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeToken : MonoBehaviour
{
    public GameObject player;
    public GameManager gameManager;
    public Rigidbody rb;

    private float pickupDist = 6;
    private float collectSpeed = 6;

    void Start()
    {
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if ((player.transform.position - transform.position).magnitude <= pickupDist)
        {
            rb.AddForce((player.transform.position - transform.position) * collectSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //collects coin
            gameManager.money++;
            Destroy(gameObject);
        }
    }
}
