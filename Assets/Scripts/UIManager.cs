using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public TMP_Text MoneyUI;
    public GameObject[] heartUIs;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Image panel;

    [HideInInspector]public bool doWhiteout;
    [HideInInspector] public bool doBlackout;

    public float transitionLenght;
    private float _panelAlpha;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        PanelAlpha();
        HealthBar();
        Money();
    }
    
    //Minimap 
    void MiniMapUpdate()
    {
        //manages camera position
        //updates position
        //blends outside
    }
    //Barre de vie joueur 
    void HealthBar()
    {
        //gerer si le coeur est actif
        for (var i = 0; i < 20; i++)
        {
            if (i > gameManager.maxHealth - 1)
            {
                heartUIs[i].GetComponent<Image>().enabled = false;
            }
            else
            {
                heartUIs[i].GetComponent<Image>().enabled = true;
                //gerer si le coeur est rempli ou pas
                for (var j = 0; j < 20; j++)
                {
                    if (j <= gameManager.health - 1)
                    {
                        heartUIs[j].GetComponent<Image>().sprite = fullHeart;
                    }
                    else
                    {
                        heartUIs[j].GetComponent<Image>().sprite = emptyHeart;
                    }
                }            
            }
        }
    }
    void Money()
    {
        //updates current money on screen
        MoneyUI.text = gameManager.money.ToString();
    }
    
    // // Slot Spell
    // void ()
    // {
    //     
    // }
    // // Slots d’Items
    // void ()
    // {
    //     
    // }

    // // Menu Pause
    // void ()
    // {
    //     
    // }
    // // Menu Principal
    // void ()
    // {
    //     
    // }
    // // Menu Options (son, résolutions)
    // void ()
    // {
    //     
    // }
    #region panel alpha
    public IEnumerator Whiteout()
    {
        _panelAlpha = 1;
        doWhiteout = true;
        yield return new WaitForSeconds(transitionLenght);
        panel.color = new Color(0, 0, 0, 0);
        doWhiteout = false;
    }
    
    public IEnumerator Blackout()
    {
        _panelAlpha = 0;
        doBlackout = true;
        yield return new WaitForSeconds(transitionLenght);
        panel.color = new Color(0, 0, 0, 1);
        doBlackout = false;
    }

    void PanelAlpha()
    {
        if (doWhiteout)
        {
            _panelAlpha -= Time.deltaTime / transitionLenght;
            panel.color = new Color(0, 0, 0, _panelAlpha);
        }

        if (doBlackout)
        {
            _panelAlpha += Time.deltaTime / transitionLenght;
            panel.color = new Color(0, 0, 0, _panelAlpha);
        }
    }
    #endregion
}
