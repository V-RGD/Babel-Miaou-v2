using System.Collections.Generic;
using UnityEngine;

public class ToNextLevel : MonoBehaviour
{
    public bool isActive;
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
                room.SetActive(false);
            }
            LevelManager.instance.roomList.Clear();
            DunGen.instance.dungeonSize = DunGen.instance.goldenPathLength;
            DunGen.instance.finishedGeneration = false;
            //builds new level
            LevelManager.instance.LoadNextLevel();
            DunGen.instance.StartCoroutine(DunGen.instance.GenPro());
            
            SaveProgression.instance.SetGameValues();
            GameMusic.instance.ChooseMusic();
            Debug.Log("nextLev");
        }
    }
}
