using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TpToBoss : MonoBehaviour
{
    private GameObject _player;
    private GameManager _gameManager;
    private LevelManager _levelManager;
    private DunGen _dunGen;
    void Start()
    {
        _gameManager = GameManager.instance;
        _dunGen = DunGen.instance;
        _player = GameObject.Find("Player");
        _levelManager = LevelManager.instance;
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_gameManager.currentLevel < 4)
            {
                //increases level
                _gameManager.currentLevel++;
                //desactivates all current rooms
                List<GameObject> oldRooms = new List<GameObject>(_levelManager.roomList);
                foreach (var room in _levelManager.roomList)
                {
                    room.SetActive(false);
                }
                _levelManager.roomList.Clear();
                _dunGen.dungeonSize = _dunGen.goldenPathLength;
                _dunGen.finishedGeneration = false;
                //builds new level
                _dunGen.StartCoroutine(_dunGen.GenPro());
                yield return new WaitUntil(()=> _dunGen.finishedGeneration);
                foreach (var room in oldRooms)
                {
                    //Destroy(room);
                }
                yield return new WaitUntil(()=> _dunGen.finishedGeneration);
                //Destroy(_dunGen.roomList);
            }
            else
            {
                SceneManager.LoadScene("BossScene");
            }
        }
    }
}
