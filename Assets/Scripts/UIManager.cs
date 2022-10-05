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
    public GameObject heartUIPrefab;
    public List<GameObject> heartUIs = new List<GameObject>(20);
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Image panel;

    [HideInInspector]public bool doWhiteout;
    [HideInInspector] public bool doBlackout;
    private GameObject canvas;

    public float transitionLenght;
    private float _panelAlpha;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        canvas = GameObject.Find("UI Canvas").transform.GetChild(3).gameObject;
    }

    void Update()
    {
        MoneyUI.text = gameManager.money.ToString();
        PanelAlpha();
        HealthBar();
    }
    
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
        //pour chaque vie, mettre un coeur offset de X par rapport au précédent
        Vector3 firstPos = new Vector3(-852,438,0);
        Vector3 offset = new Vector3(0, 0, -200);
        //pour chaque point de vie, on crée un coeur
        for (int i = 0; i < gameManager.maxHealth; i++)
        {
            Debug.Log(i);
            if (heartUIs[i] == null)
            {
                GameObject heart = Instantiate(heartUIPrefab, canvas.transform);
                heart.transform.position = firstPos + (offset * i);
            }
        }
        //pour chaque point de vie max, créer un coeur
        for (int i = 0; i < gameManager.maxHealth; i++)
        {
            if (i <= gameManager.health)
            {
                heartUIs[i].GetComponent<SpriteRenderer>().sprite = fullHeart;
            }
            else
            {
                heartUIs[i].GetComponent<SpriteRenderer>().sprite = emptyHeart;
            }
        }
    }
    // //Thunes
    // void Money()
    // {
    //     
    // }
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
    //
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
}
