using System;
using System.Linq;
using UnityEngine;

public class EyeToken : MonoBehaviour
{
    public GameObject player;
    private GameManager _gameManager;
    public Rigidbody rb;

    private float _pickupDist = 6;
    private float _collectSpeed = 6;
    private bool _canMoveToPlayer;
    private GameObject _littleShit;
    
    void Start()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        _littleShit = ObjectsManager.instance.eyeCollector;
        
        {
            GameManager.instance.eyesInGame.Add(gameObject.transform);
            //sort list
            GameManager.instance.eyesInGame = GameManager.instance.eyesInGame.OrderBy( point => Vector3.Distance(player.transform.position,point.position)).ToList();
        }
    }

    private void Update()
    {
        if ((player.transform.position - transform.position).magnitude < _pickupDist)
        {
            GoToPlayer();
        }

        if ((_littleShit.transform.position - transform.position).magnitude < _pickupDist)
        {
            GoToRoger();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //player collects coin
            _gameManager.money++;
            _gameManager.eyesInGame.Remove(transform);
            PlayerSounds.instance.eyeSource.PlayOneShot(PlayerSounds.instance.eyeSource.clip);
            Destroy(gameObject);
        }
        if (other.CompareTag("LittleShit"))
        {
            LittleShit.instance.animator.CrossFade(LittleShit.instance.Eat, 0, 0);
            //enemy collects it
            LittleShit.instance.eyesInInventory++;
            _gameManager.eyesInGame.Remove(transform);
            _gameManager.eyesInGame.Remove(transform);
            Destroy(gameObject);
            ObjectsManager.instance.PlayActivationVfx(3);
        }
    }

    void GoToPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.1f);
    }
    
    void GoToRoger()
    {
        transform.position = Vector3.MoveTowards(transform.position, _littleShit.transform.position, 0.1f);
    }
}
