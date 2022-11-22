using UnityEngine;

public class HugeLaserDamage : MonoBehaviour
{
    private GameManager _gameManager;
    public float damage;
    private GameObject _player;

    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //deals damage
        if (other.CompareTag("Player") && _player.GetComponent<PlayerController>().stunCounter < 0 && !_player.GetComponent<PlayerController>()._playerAttacks.isAttacking)
        {
            _gameManager.DealDamageToPlayer(damage);
        }
    }
}
