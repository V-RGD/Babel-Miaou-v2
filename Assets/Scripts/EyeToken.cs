using System;
using System.Linq;
using UnityEngine;

public class EyeToken : MonoBehaviour
{
    public GameObject player;
    private GameManager _gameManager;
    public Rigidbody rb;

    private float pickupDist = 6;
    private float collectSpeed = 6;
    
    void Start()
    {
        player = GameObject.Find("Player");
        _gameManager = GameManager.instance;
        rb = GetComponent<Rigidbody>();
        
        {
            _gameManager.eyesInGame.Add(gameObject.transform);
            //sort list
            _gameManager.eyesInGame = _gameManager.eyesInGame.OrderBy( point => Vector3.Distance(player.transform.position,point.position)).ToList();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //player collects coin
            _gameManager.money++;
            _gameManager.eyesInGame.Remove(transform);
            Destroy(gameObject);
        }
        if (other.CompareTag("LittleShit"))
        {
            //enemy collects it
            other.GetComponent<LittleShit>().eyesInInventory++;
            _gameManager.eyesInGame.Remove(transform);
            _gameManager.eyesInGame.Remove(transform);
            Destroy(gameObject);
        }
    }
}
