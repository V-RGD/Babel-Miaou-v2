using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusic : MonoBehaviour
{
    public static GameMusic instance;
    
    public AudioClip[] musics;
    private AudioSource source;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
        
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
    }

    public void ChooseMusic()
    {
        if (MenuManager.instance.gameState == MenuManager.GameState.MainMenu)
        {
            source.clip = musics[0];
            source.Play();
            return;
        }
        
        switch (LevelManager.instance.currentLevel)
        {
            case 0 :
                source.clip = musics[1];
                source.Play();
                break;
            case 1 : 
                source.clip = musics[2];
                source.Play();
                break;
            case 2 : 
                source.clip = musics[3];
                source.Play();
                break;
        }
    }
}
