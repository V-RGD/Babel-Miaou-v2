using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    private GameManager gameManager;
    private PlayerController _playerController;
    
    public PlayerControls playerControls;
    private ObjectsManager _objectsManager;
    private InputAction mouseClick;
    public TMP_Text MoneyUI;
    public GameObject[] heartUIs;
    public GameObject[] itemBoxHUD;
    public GameObject[] enemyWarnings;
    public Transform enemyWarningGroup;
    public Sprite fullHeart;
    public Sprite midHeart;
    public Sprite emptyHeart;
    public Image panel;
    //public Image smashSlider;
    public Animator hurtPanel;
    public RectTransform lowHpPanel;

    [HideInInspector] public bool doWhiteout;
    [HideInInspector] public bool doBlackout;

    public float transitionLenght;
    private float _panelAlpha;

    public bool canEscapeObjectMenu = true;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        instance = this;
        
        playerControls = new PlayerControls();
        mouseClick = playerControls.Menus.MouseClick;
    }

    private void Start()
    {
        _objectsManager = ObjectsManager.instance;
        _playerController = PlayerController.instance;
        gameManager = GameManager.instance;
    }

    void Update()
    {
        PanelAlpha();
        Money();
        SmashGauge();
    }
    //Barre de vie joueur 
    public void HurtPanels()
    {
        //launches hurt anim
        hurtPanel.SetTrigger("RedOut");
        //updates low hp panel scale
        float health = gameManager.health;
        float maxHealth = gameManager.maxHealth;
        if (health/maxHealth < 0.5f)
        {
            float panelScale = 1 + (3 * (health/maxHealth));
            lowHpPanel.localScale = new Vector3(panelScale, panelScale, 1);
        }
        else
        {
            lowHpPanel.localScale = new Vector3(4, 4, 1);
        }
    }
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
        // //used to display progress bar for smash holding
        // float value = _playerController._playerAttacks.smashGauge * 100;
        // float xValue = value / 2 - 50;
        //
        // if (value > 20)
        // {
        //     smashSlider.gameObject.SetActive(true);
        //     smashSlider.GetComponent<RectTransform>().localPosition = new Vector3(xValue, 0, 0);
        //     smashSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(value, 10);
        //     smashSlider.transform.parent.transform.GetChild(0).gameObject.SetActive(true);
        // }
        // else
        // {
        //     smashSlider.gameObject.SetActive(false);
        //     smashSlider.transform.parent.transform.GetChild(0).gameObject.SetActive(false);
        // }
    }

    public void UpdateHUDIcons()
    {
        //used to reload data from the object when added
        for (int i = 0; i < 3; i++)
        {
            if (_objectsManager.itemObjectsInventory[i] != 999)
            {
                int id = _objectsManager.itemObjectsInventory[i];
                //update : box name, icon, description, rarity color
                Sprite icon = _objectsManager.objectSprites[id];
                itemBoxHUD[i].transform.GetChild(2).GetComponent<Image>().enabled = true;
                itemBoxHUD[i].transform.GetChild(2).GetComponent<Image>().sprite = icon;
            }
            else
            {
                //shows empty box
                itemBoxHUD[i].transform.GetChild(2).GetComponent<Image>().sprite = null;
                itemBoxHUD[i].transform.GetChild(2).GetComponent<Image>().enabled = false;
            }
        }
    }
}
