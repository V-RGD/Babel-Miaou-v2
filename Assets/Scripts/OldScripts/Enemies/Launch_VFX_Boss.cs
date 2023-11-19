using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Launch_VFX_Boss : MonoBehaviour
{
    [Header("Anticipation")]
    public ParticleSystem pc;
    
    [Header("Drop")]
    public float delay1;
    public ParticleSystem pc2;
    public VisualEffect fx;

    [Header("Impact")]
    public float delay2;
    public ParticleSystem pc3;
    //public AnimationClip anim1;

    [Header("After_Impact")]
    public float delay3;
    public ParticleSystem pc4;

    public IEnumerator BossFxSequence()
    {
        pc.Play();
        yield return new WaitForSeconds(delay1); 
        pc2.Play();
        fx.Play();
        yield return new WaitForSeconds(delay2);
        pc3.Play();
        GameManager_old.instance.hurtVolume.CrossFade(Animator.StringToHash("Hurt"), 0);
        //anim1.Play();
        yield return new WaitForSeconds(delay3);
        pc4.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(BossFxSequence());
        }
    }
}


