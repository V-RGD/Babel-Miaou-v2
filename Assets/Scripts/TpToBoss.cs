using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TpToBoss : MonoBehaviour
{
    private GameObject _player;
    void Start()
    {
        _player = GameObject.Find("Player");
    }

    private void OnTriggerEnter(Collider other)
    { 
        //if finds player, tp to boss room
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("BossRoom");
        }
    }
}
