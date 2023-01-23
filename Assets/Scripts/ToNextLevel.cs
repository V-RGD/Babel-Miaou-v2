using UnityEngine;

public class ToNextLevel : MonoBehaviour
{
    public bool isActive;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && isActive)
        {
            LevelManager.instance.LoadNextLevel();
            GameMusic.instance.ChooseMusic();
            Debug.Log("nextLev");
        }
    }
}
