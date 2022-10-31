using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerController _playerController;
    
    public PlayerControls playerControls;
    private InputAction mouseClick;
    public TMP_Text MoneyUI;
    public GameObject[] heartUIs;
    public Sprite fullHeart;
    public Sprite midHeart;
    public Sprite emptyHeart;
    public Image panel;
    public Image smashSlider;

    [HideInInspector] public bool doWhiteout;
    [HideInInspector] public bool doBlackout;
    [HideInInspector] public bool isMapHighlight;
    [HideInInspector] public bool isMapFull;

    public float transitionLenght;
    private float _panelAlpha;

    public bool canEscapeObjectMenu = true;

    private void Awake()
    {
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerControls = new PlayerControls();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mouseClick = playerControls.Menus.MouseClick;
    }

    void Update()
    {
        PanelAlpha();
        Money();
        SmashGauge();
    }
    //Barre de vie joueur 
    public void HealthBar(int health)
    {
        //gerer si le coeur est actif
        for (var i = 0; i < 20; i++)
        {
            Image image = heartUIs[i].GetComponent<Image>();
            int ceil = i * 2 + 1;
            if (gameManager.maxHealth >= ceil)
            {
                if (health < ceil)
                {
                    //si la vie est en dessous du seuil requis pour l'activation du coeur, le desactiver
                    image.sprite = emptyHeart;
                }
                else
                {
                    image.enabled = true;
                    if (health == ceil)
                    {
                        image.sprite = midHeart;
                    }

                    if (health > ceil)
                    {
                        image.sprite = fullHeart;
                    }
                }
            }
            else
            {
                image.enabled = false;  
            }
        }
    }
    void Money()
    {
        //updates current money on screen
        MoneyUI.text = gameManager.money.ToString();
    }
    
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

    private void OnEnable()
    {
        mouseClick.Enable();
        mouseClick.performed += _ => canEscapeObjectMenu = false;
        mouseClick.canceled += _ => canEscapeObjectMenu = true;
    }
    
    private void OnDisable()
    {
        mouseClick.Disable();
    }

    void SmashGauge()
    {
        //used to display progress bar for smash holding
        float value = _playerController.smashGauge * 100;
        float xValue = value / 2 - 50;

        if (value > 20)
        {
            smashSlider.gameObject.SetActive(true);
            smashSlider.GetComponent<RectTransform>().localPosition = new Vector3(xValue, 0, 0);
            smashSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(value, 10);
            smashSlider.transform.parent.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            smashSlider.gameObject.SetActive(false);
            smashSlider.transform.parent.transform.GetChild(0).gameObject.SetActive(false);
        }
        
    }
}
