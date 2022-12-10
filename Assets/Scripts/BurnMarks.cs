using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnMarks : MonoBehaviour
{
    //for burn marks on the floor
    private PlayerAttacks _playerAttacks;
    private GameObject _player;
    private Transform _gameManager;
    
    [SerializeField]private GameObject semiBurntVfx;
    [SerializeField]private GameObject reverseBurntVfx;
    [SerializeField]private GameObject fullBurntVfx;

    private List<ParticleSystem> _semiBurntList = new List<ParticleSystem>();
    private List<ParticleSystem> _reverseBurntList = new List<ParticleSystem>();
    private List<ParticleSystem> _fullBurntList = new List<ParticleSystem>();

    private float _semiBurntAmount = 20f;
    private float _reverseBurntAmount = 20f;
    private float _fullBurntAmount = 20f;

    private int _semiBurntCounter;
    private int _reverseBurntCounter;
    private int _fullBurntCounter;

    public Vector3 attackDir;
    public float offset;
    public float duration;

    private void Start()
    {
        _playerAttacks = PlayerAttacks.instance;
        _gameManager = GameObject.Find("GameManager").transform;
        _player = GameObject.Find("Player");
        
        for (int i = 0; i < _semiBurntAmount; i++)
        {
            GameObject semi = Instantiate(semiBurntVfx, Vector3.back * 1000, Quaternion.identity);
            _semiBurntList.Add(semi.GetComponent<ParticleSystem>());
            semi.transform.parent = _gameManager.transform;
        }
        
        for (int i = 0; i < _reverseBurntAmount; i++)
        {
            GameObject reverse = Instantiate(reverseBurntVfx, Vector3.back * 1000, Quaternion.identity);
            _reverseBurntList.Add(reverse.GetComponent<ParticleSystem>());
            reverse.transform.parent = _gameManager.transform;
        }
        
        for (int i = 0; i < _fullBurntAmount; i++)
        {
            GameObject full = Instantiate(fullBurntVfx, Vector3.back * 1000, Quaternion.identity);
            _fullBurntList.Add(full.GetComponent<ParticleSystem>());
            full.transform.parent = _gameManager.transform;
        }
    }

    public IEnumerator PlaceNewVfx(int type)
    {
        //place vfx
        GameObject vfx = _semiBurntList[_semiBurntCounter].gameObject;
        ParticleSystem particle = _semiBurntList[_semiBurntCounter];
        //selects type of vfx used
        switch (type)
        {
            case 0 :
                vfx = _semiBurntList[_semiBurntCounter].gameObject;
                particle = _semiBurntList[_semiBurntCounter];
                if (_semiBurntCounter < _semiBurntList.Count - 1)
                {
                    _semiBurntCounter++;
                }
                else
                {
                    _semiBurntCounter = 0;
                }
                break;
            case 1 :
                vfx = _reverseBurntList[_reverseBurntCounter].gameObject;
                particle = _reverseBurntList[_reverseBurntCounter];
                if (_reverseBurntCounter < _reverseBurntList.Count - 1)
                {
                    _reverseBurntCounter++;
                }
                else
                {
                    _reverseBurntCounter = 0;
                }
                break;
            case 2 :
                vfx = _fullBurntList[_fullBurntCounter].gameObject;
                particle = _fullBurntList[_fullBurntCounter];
                if (_fullBurntCounter < _fullBurntList.Count - 1)
                {
                    _fullBurntCounter++;
                }
                else
                {
                    _fullBurntCounter = 0;
                }
                break;
        }
        //sets position and rotation according to the player direction
        vfx.transform.position = _player.transform.position;
        vfx.transform.LookAt(_player.transform.position + (-attackDir * 1000));
        vfx.transform.position += attackDir * (5 * offset);
        //actives fx
        particle.Stop();
        particle.Play();
        yield return new WaitForSeconds(duration);
        //dissapears far away
        vfx.transform.position = Vector3.back * 1000;
    }
}
