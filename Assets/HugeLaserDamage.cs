using UnityEngine;

public class HugeLaserDamage : MonoBehaviour
{
    private GameManager _gameManager;
    public float damage;

    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //_gameManager.DealDamageToPlayer(damage);
    }
}
