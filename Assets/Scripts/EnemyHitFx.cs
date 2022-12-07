using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitFx : MonoBehaviour
{
    //for burn marks on the floor
    private PlayerAttacks _playerAttacks;
    private GameObject _player;
    [SerializeField]private GameObject hitFx;
    [SerializeField]private List<ParticleSystem> _hitFxList = new List<ParticleSystem>();
    [SerializeField]private float _hitFxAmount = 100f;
    public float duration;
    [SerializeField]private int _hitFxCounter;

    private void Start()
    {
        _playerAttacks = PlayerAttacks.instance;
        _player = GameObject.Find("Player");
        
        for (int i = 0; i < _hitFxAmount; i++)
        {
            GameObject fx = Instantiate(hitFx, Vector3.back * 1000, Quaternion.identity);
            _hitFxList.Add(fx.GetComponent<ParticleSystem>());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(PlaceNewVfx(_player.transform.position));
        }
    }

    public IEnumerator PlaceNewVfx(Vector3 enemyPos)
    {
        //place vfx
        GameObject vfx = _hitFxList[_hitFxCounter].gameObject;
        ParticleSystem particle = _hitFxList[_hitFxCounter];
        if (_hitFxCounter < _hitFxList.Count - 1)
        {
            _hitFxCounter++;
        }
        else
        {
            _hitFxCounter = 0;
        }
        
        //sets position in front of enemy
        Vector3 position = new Vector3(enemyPos.x, _player.transform.position.y, enemyPos.z);
        vfx.transform.position = position + (_player.transform.position - position).normalized * 0;
        vfx.SetActive(true);
        //set rotation as the result of the attack
        vfx.transform.LookAt(position + (_player.transform.position - position).normalized * 1000);
        //actives fx
        particle.Stop();
        particle.Play();
        yield return new WaitForSeconds(duration);
        //dissapears far away
        vfx.transform.position = Vector3.back * 1000;
        vfx.SetActive(false);
    }
}
