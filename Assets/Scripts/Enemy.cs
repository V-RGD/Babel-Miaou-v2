using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject spawnZone;
    private SpriteRenderer _spriteRenderer;
    
    public bool startSpawning;
    private bool canInitiateSpawning = true;
    // Start is called before the first frame update
    void Start()
    {
        spawnZone = transform.GetChild(0).gameObject;
        _spriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        GetComponent<EnemyBehaviour>().enabled = false;
        _spriteRenderer.enabled = false;
        spawnZone.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning && canInitiateSpawning)
        {
            canInitiateSpawning = false;
            StartCoroutine(EnemyApparition());
        }
    }

    IEnumerator EnemyApparition()
    {
        //spawn zone appears
        spawnZone.SetActive(true);
        //vfx plays
        yield return new WaitForSeconds(2);
        //then enemy spawns
        spawnZone.SetActive(false);
        GetComponent<EnemyBehaviour>().enabled = true;
        _spriteRenderer.enabled = true;
    }
}
