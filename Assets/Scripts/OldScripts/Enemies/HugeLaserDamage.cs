using UnityEngine;

public class HugeLaserDamage : MonoBehaviour
{
    private GameManager_old _gameManager;
    public float damage;
    private GameObject _player;

    private void Start() => _gameManager = GameManager_old.instance;


    private void OnTriggerEnter(Collider other)
    {
        //deals damage
        if (other.CompareTag("Player") && PlayerController__old.instance.stunCounter < 0 && !PlayerAttacks_old.instance.isAttacking)
        {
            _gameManager.DealDamageToPlayer(damage);
        }
    }
}
