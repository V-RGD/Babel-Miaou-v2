using System.Collections;
using UnityEngine;

public class BreakablePot : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private SpriteRenderer _sr;
    private BoxCollider _collider;

    private void Awake()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _sr = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
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
