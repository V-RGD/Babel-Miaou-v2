using System;
using System.Collections.Generic;
using UnityEngine;

public class ToNextLevel : MonoBehaviour
{
    public bool isActive;
    public AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _audioSource.PlayOneShot(GameSounds.instance.bossRock[0]);
        GameManager.instance.cmShake.ShakeCamera(7, 0.1f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && isActive)
        {
            //increases level
            GameManager.instance.currentLevel++;
            //desactivates all current rooms
            List<GameObject> oldRooms = new List<GameObject>(LevelManager.instance.roomList);
            foreach (var room in LevelManager.instance.roomList)
            {
                Destroy(room);
            }
            LevelManager.instance.roomList.Clear();
            DunGen.instance.dungeonSize = DunGen.instance.goldenPathLength;
            DunGen.instance.finishedGeneration = false;
            //builds new level
            LevelManager.instance.LoadNextLevel();
            
            SaveProgression.instance.SetGameValues();
            GameMusic.instance.ChooseMusic();
            Debug.Log("nextLev");
        }
    }
}
