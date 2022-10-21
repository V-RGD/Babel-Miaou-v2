using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EyeToken : MonoBehaviour
{
    public GameObject player;
    public GameManager gameManager;
    public Rigidbody rb;

    private float pickupDist = 6;
    private float collectSpeed = 6;

    private LittleShit _littleShit;

    void Start()
    {
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        
        if (GameObject.Find("LittleShit") != null)
        {
            _littleShit = GameObject.Find("LittleShit").GetComponent<LittleShit>();
            _littleShit.eyesInGame.Add(gameObject.transform);
            //sort list
            _littleShit.eyesInGame = _littleShit.eyesInGame.OrderBy( point => Vector3.Distance(player.transform.position,point.position)).ToList();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //player collects coin
            gameManager.money++;
            if (GameObject.Find("LittleShit") != null)
            {
                _littleShit.eyesInGame.Remove(transform);
            }
            Destroy(gameObject);
        }
        if (other.CompareTag("LittleShit"))
        {
            //enemy collects it
            other.GetComponent<LittleShit>().eyesInInventory++;
            _littleShit.eyesInGame.Remove(transform);
            if (GameObject.Find("LittleShit") != null)
            {
                _littleShit.eyesInGame.Remove(transform);
            }
            Destroy(gameObject);
        }
    }

    void PosRandomizer()
    {
        
    }
}
