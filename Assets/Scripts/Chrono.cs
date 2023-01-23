using UnityEngine;

public class Chrono : MonoBehaviour
{
    public static Chrono instance;
    public float elapsedTime;

    private void Awake()
    {
        if (instance != null)
        {
            //Destroy(gameObject);
        }

        instance = this;
    }

    private void Start()
    {
        elapsedTime = 0;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
    }
}
