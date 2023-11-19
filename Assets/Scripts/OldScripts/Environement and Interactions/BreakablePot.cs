using System.Collections;
using UnityEngine;

public class BreakablePot : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private SpriteRenderer _sr;
    private BoxCollider _collider;
    private bool _canBeDestroyed = true;

    private void Awake()
    {
        _particleSystem = transform.GetChild(1).GetComponent<ParticleSystem>();
        _sr = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider>();
        _canBeDestroyed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack") && _canBeDestroyed)
        {
            _canBeDestroyed = false;
            StartCoroutine(Break());
        }
    }

    IEnumerator Break()
    {
        _particleSystem.Play();
        _sr.enabled = false;
        _collider.enabled = false;

        yield return new WaitForSeconds(_particleSystem.main.startLifetime.constantMax);
        Destroy(gameObject);
    }
}
