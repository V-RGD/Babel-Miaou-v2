using UnityEngine;

public class HugeLaserDamage : MonoBehaviour
{
    private GameManager _gameManager;
    public float damage;
    private GameObject _player;

    private void Start() => _gameManager = GameManager.instance;


    private void OnTriggerEnter(Collider other)
    {
        //deals damage
        if (other.CompareTag("Player") && PlayerController.instance.stunCounter < 0 && !PlayerAttacks.instance.isAttacking)
        {
            _gameManager.DealDamageToPlayer(damage);
        }
    }
}
