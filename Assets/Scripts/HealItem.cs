using UnityEngine;

public class HealItem : MonoBehaviour
{
    public GameManager gameManager;
    public int healAmount;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.health += healAmount;
            Destroy(gameObject);
        }
    }
}
