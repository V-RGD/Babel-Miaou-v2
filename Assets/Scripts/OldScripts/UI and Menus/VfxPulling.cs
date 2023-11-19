using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class VfxPulling : MonoBehaviour
{
    [Header("CATEGORY")]
    public string name;
    //for burn marks on the floor
    private PlayerAttacks_old _playerAttacks;
    private GameObject _player;
    private Transform _gameManager;
    
    [Serializable]
    public class Vfx
    {
        public VisualEffect effect;
        public int generatedAmount = 20;
        [HideInInspector]public int counter;
        [HideInInspector] public List<VisualEffect> effectList = new List<VisualEffect>();
        public float offset;
        public float duration;
    }
    
    [Serializable]
    public class Particle
    {
        public ParticleSystem particle;
        public int generatedAmount = 20;
        [HideInInspector]public int counter;
        [HideInInspector]public List<ParticleSystem> particleList = new List<ParticleSystem>();
        public float offset;
        public float duration;
    }
    public List<Particle> particleList;
    public List<Vfx> vfxList;
    [HideInInspector] public Vector3 attackDir;

    private void Start()
    {
        _playerAttacks = PlayerAttacks_old.instance;
        _gameManager = GameManager_old.instance.transform;
        _player = GameObject.Find("Player");

        foreach (var particleType in particleList)
        {
            for (int i = 0; i < particleType.generatedAmount; i++)
            {
                GameObject newParticle = Instantiate(particleType.particle.gameObject, Vector3.back * 1000, Quaternion.identity);
                newParticle.SetActive(false);
                particleType.particleList.Add(newParticle.GetComponent<ParticleSystem>());
                newParticle.transform.parent = _gameManager.transform;
            }
        }
        
        foreach (var effectType in vfxList)
        {
            for (int i = 0; i < effectType.generatedAmount; i++)
            {
                GameObject newParticle = Instantiate(effectType.effect.gameObject, Vector3.back * 1000, Quaternion.identity);
                newParticle.SetActive(false);
                effectType.effectList.Add(newParticle.GetComponent<VisualEffect>());
                newParticle.transform.parent = _gameManager.transform;
            }
        }
    }

    public void PlaceBurnMark(int type)
    {
        //place vfx
        Particle particle = particleList[0];
        //selects type of vfx used
        switch (type)
        {
            case 0 :
                particle = particleList[0];
                break;
            case 1 :
                particle = particleList[1];
                break;
            case 2 :
                particle = particleList[2];
                break;
        }
        StartCoroutine(PlaceNewVfx(particle, true));
        PlayerSounds.instance.PlayAttackSound(type);
    }
    
    public IEnumerator PlaceNewVfx(Particle newParticle)
    {
        //place vfx
        ParticleSystem particle = newParticle.particleList[newParticle.counter];
        if (newParticle.counter < newParticle.particleList.Count - 1)
        {
            newParticle.counter++;
        }
        else
        {
            newParticle.counter = 0;
        }

        //sets position and rotation according to the player direction
        Vector3 pos = new Vector3(transform.position.x, 0.2f, transform.position.z);
        particle.transform.position = pos;
        particle.transform.position += attackDir * newParticle.offset;
        //actives fx
        particle.gameObject.SetActive(true);
        particle.Stop();
        particle.Play();
        yield return new WaitForSeconds(newParticle.duration);
        particle.gameObject.SetActive(false);
        //dissapears far away
        particle.transform.position = Vector3.back * 10000;
    }
    //for smash
    public IEnumerator PlaceNewVfx(Vfx newVfx)
    {
        //place vfx
        VisualEffect particle = newVfx.effectList[newVfx.counter];
        if (newVfx.counter < newVfx.effectList.Count - 1)
        {
            newVfx.counter++;
        }
        else
        {
            newVfx.counter = 0;
        }
        //sets position and rotation according to the player direction
        Vector3 pos = new Vector3(transform.position.x, 0.2f, transform.position.z);
        particle.transform.position = pos;
        particle.transform.position += attackDir * newVfx.offset;
        //actives fx
        particle.gameObject.SetActive(true);
        particle.Stop();
        particle.Play();
        yield return new WaitForSeconds(newVfx.duration);
        particle.gameObject.SetActive(false);
        //dissapears far away
        particle.transform.position = Vector3.back * 10000;
    }
    public IEnumerator PlaceNewVfx(Particle newParticle, bool directedByRotation)
    {
        //place vfx
        ParticleSystem particle = newParticle.particleList[newParticle.counter];
        if (newParticle.counter < newParticle.particleList.Count - 1)
        {
            newParticle.counter++;
        }
        else
        {
            newParticle.counter = 0;
        }

        //sets position and rotation according to the player direction
        Vector3 pos = new Vector3(transform.position.x, 0.2f, transform.position.z);
        particle.transform.position = pos;
        float randRot = Random.Range(-30f, 30f);
        Vector3 randomRotation = new Vector3(randRot ,0, randRot);
        particle.transform.LookAt(_player.transform.position + (-attackDir * 1000 + randomRotation * 10) );
        particle.transform.position += attackDir * newParticle.offset;
        //actives fx
        particle.gameObject.SetActive(true);
        particle.Stop();
        particle.Play();
        yield return new WaitForSeconds(newParticle.duration);
        particle.gameObject.SetActive(false);
        //disapears far away
        particle.transform.position = Vector3.back * 10000;
    }
    public IEnumerator PlaceNewVfx(Particle newParticle, Vector3 rotation)
    {
        //place vfx
        ParticleSystem particle = newParticle.particleList[newParticle.counter];
        if (newParticle.counter < newParticle.particleList.Count - 1)
        {
            newParticle.counter++;
        }
        else
        {
            newParticle.counter = 0;
        }
        //sets position and rotation according to the player direction
        Vector3 pos = new Vector3(transform.position.x, 0.2f, transform.position.z);
        particle.transform.position = pos;
        particle.transform.LookAt(_player.transform.position + (-rotation * 1000));
        particle.transform.position += attackDir * newParticle.offset;
        //actives fx
        particle.gameObject.SetActive(true);
        particle.Stop();
        particle.Play();
        yield return new WaitForSeconds(newParticle.duration);
        particle.gameObject.SetActive(false);
        //dissapears far away
        particle.transform.position = Vector3.back * 10000;
    }
    public void PlaceDashFx()
    {
        //place vfx
        Particle particle = particleList[3];
        StartCoroutine(PlaceNewVfx(particle, new Vector3(PlayerController__old.instance.lastWalkedDir.x, 0 ,PlayerController__old.instance.lastWalkedDir.y)));
    }
    
    public IEnumerator PlaceNewVfx(Particle newParticle, Vector3 position, bool isPlacedByPosition)
    {
        //place vfx
        ParticleSystem particle = newParticle.particleList[newParticle.counter];
        if (newParticle.counter < newParticle.particleList.Count - 1)
        {
            newParticle.counter++;
        }
        else
        {
            newParticle.counter = 0;
        }
        //sets position and rotation according to the player direction
        Vector3 pos = new Vector3(position.x, 0.2f, position.z);
        //Debug.Log(pos);
        particle.transform.position = pos;
        //actives fx
        particle.gameObject.SetActive(true);
        particle.Stop();
        particle.Play();

        yield return new WaitForSeconds(newParticle.duration);
        particle.gameObject.SetActive(false);
        //dissapears far away
        particle.transform.position = Vector3.back * 10000;
    }
    
    public IEnumerator PlaceNewVfx(Vfx newVfx, Vector3 position, bool isPlacedByPosition)
    {
        //place vfx
        VisualEffect particle = newVfx.effectList[newVfx.counter];
        if (newVfx.counter < newVfx.effectList.Count - 1)
        {
            newVfx.counter++;
        }
        else
        {
            newVfx.counter = 0;
        }
        //sets position and rotation according to the player direction
        Vector3 pos = new Vector3(position.x, 0.2f, position.z);
        //Debug.Log(pos);
        particle.transform.position = pos;
        //actives fx
        particle.gameObject.SetActive(true);
        particle.Stop();
        particle.Play();
        yield return new WaitForSeconds(newVfx.duration);
        particle.gameObject.SetActive(false);
        //dissapears far away
        particle.transform.position = Vector3.back * 10000;
    }
}
