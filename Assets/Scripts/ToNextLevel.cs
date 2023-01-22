using UnityEngine;

public class ToNextLevel : MonoBehaviour
{
    private void Awake()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.instance.LoadNextLevel();
            GameMusic.instance.ChooseMusic();
            Debug.Log("nextLev");
        }
    }
}
