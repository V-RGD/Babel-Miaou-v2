using UnityEngine;

public class ToNextLevel : MonoBehaviour
{
    private LevelManager _lm;

    private void Awake()
    {
        _lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _lm.LoadNextLevel();
            Debug.Log("nextLev");
        }
    }
}
