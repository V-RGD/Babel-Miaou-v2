using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VfxPulling : MonoBehaviour
{
    //for burn marks on the floor
    private PlayerAttacks _playerAttacks;
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
        _playerAttacks = PlayerAttacks.instance;
        _gameManager = GameManager.instance.transform;
        _player = GameObject.Find("Player");

        foreach (var particleType in particleList)
        {
            for (int i = 0; i < particleType.generatedAmount; i++)
            {
                GameObject newParticle = Instantiate(particleType.particle.gameObject, Vector3.back * 1000, Quaternion.identity);
                particleType.particleList.Add(newParticle.GetComponent<ParticleSystem>());
                newParticle.transform.parent = _gameManager.transform;
            }
        }
        
        foreach (var effectType in vfxList)
        {
            for (int i = 0; i < effectType.generatedAmount; i++)
            {
                GameObject newParticle = Instantiate(effectType.effect.gameObject, Vector3.back * 1000, Quaternion.identity);
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
        //particle.transform.LookAt(_player.transform.position + (-attackDir * 1000));
        particle.transform.position += attackDir * newParticle.offset;
        //actives fx
        particle.Stop();
        particle.Play();
        yield return new WaitForSeconds(newParticle.duration);
        //dissapears far away
        particle.transform.position = Vector3.back * 1000;
    }
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
        //particle.transform.LookAt(_player.transform.position + (-attackDir * 1000));
        particle.transform.position += attackDir * newVfx.offset;
        //actives fx
        particle.Stop();
        particle.Play();
        yield return new WaitForSeconds(newVfx.duration);
        //dissapears far away
        particle.transform.position = Vector3.back * 1000;
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
        particle.transform.LookAt(_player.transform.position + (-attackDir * 1000));
        particle.transform.position += attackDir * newParticle.offset;
        //actives fx
        particle.Stop();
        particle.Play();
        yield return new WaitForSeconds(newParticle.duration);
        //dissapears far away
        particle.transform.position = Vector3.back * 1000;
    }
}
