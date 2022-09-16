using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public TMP_Text HeightUI;
    public TMP_Text CurrentLevelUI;
    
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        HeightUI.text = gameManager.height + "m";
        CurrentLevelUI.text = "Level " + gameManager.currentLevel;
    }
}
