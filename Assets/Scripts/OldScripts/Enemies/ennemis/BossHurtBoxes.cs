using System.Collections;
using UnityEngine;

public class BossHurtBoxes : MonoBehaviour
{
    public bool isMain;
    public bool isActive;
    public int health = 30;
    public float respawnCooldown;
    public float respawnTimer;
    private GameManager_old _gameManager;
    public GameObject visuals;
    private void Start() => _gameManager = GameManager_old.instance;

    private void OnTriggerEnter(Collider other)
    {
        //if player hit
        if (other.CompareTag("PlayerAttack"))
        {
            TakeDamage(other.GetComponent<ObjectDamage>().damage);
        }
    }

    private void Update()
    {
        if (health < 0 && !isMain)
        {
            isActive = false;
            visuals.SetActive(false);
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        //waits until respawn
        yield return new WaitForSeconds(30);
        isActive = true;
        visuals.SetActive(true);
    }

    void TakeDamage(float damageDealt) //when enemy takes hit
    {
        //clamps damage to an int (security)
        int damage = Mathf.CeilToInt(damageDealt);
        //applies damage
        health -= damage;
        _gameManager.cmShake.ShakeCamera(5, .1f);
        
        switch (PlayerAttacks_old.instance.comboState)
        {
            case PlayerAttacks_old.ComboState.SimpleAttack:
                _gameManager.cmShake.ShakeCamera(2, .1f);
                break;
            case PlayerAttacks_old.ComboState.ReverseAttack:
                _gameManager.cmShake.ShakeCamera(2, .1f);
                break;
            case PlayerAttacks_old.ComboState.SpinAttack:
                _gameManager.cmShake.ShakeCamera(2, .1f);
                break;
        }
    }
}
