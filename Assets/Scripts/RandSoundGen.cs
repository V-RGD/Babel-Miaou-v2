using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandSoundGen : MonoBehaviour
{
    private AudioSource _outputSource;
    public string soundType;
    ///declare sound type
    [Serializable]
    public class SoundType
    {
        //original sounds
        public List<AudioClip> sounds = new List<AudioClip>();
        //to avoid ear tiredness, takes a random sound from the remaining ones, then removes it from the list
        [HideInInspector] public List<AudioClip> remainingSounds = new List<AudioClip>();
    }
    public List<SoundType> typeList;

    public void Awake()
    {
        _outputSource = GetComponent<AudioSource>();
    }

    public void PlayRandomSound(int type)
    {
        //check if empty
        if (typeList[type].remainingSounds.Count == 0)
        {
            typeList[type].remainingSounds = new List<AudioClip>(typeList[type].sounds);
        }
        //takes the right sound list
        List<AudioClip> soundList = typeList[type].remainingSounds;
        //takes a random sound from the list
        int randInt = Random.Range(0, soundList.Count);
        AudioClip clip = soundList[randInt];
        //removes it to shuffle
        soundList.Remove(soundList[randInt]);
        //assigns, then plays sound
        if (_outputSource == null)
        {
            return;
        }
        
        _outputSource.clip = clip;
        _outputSource.PlayOneShot(_outputSource.clip);
    }
}
