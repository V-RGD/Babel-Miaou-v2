using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private PlayerControls _playerControls;
    private InputAction mouseClick;
    public TMP_Text MoneyUI;
    public GameObject[] itemBoxHUD;
    public GameObject[] enemyWarnings;
    public Transform enemyWarningGroup;
    public Image panel;
    public Animator hurtPanel;
    public RectTransform lowHpPanel;
    public RectTransform healthBarAmount;

    [HideInInspector] public bool doWhiteout;
    [HideInInspector] public bool doBlackout;

    public float transitionLenght;
    private float _panelAlpha;
    
    public RectTransform healthBarFill;
    public RectTransform healthBarFlare;
    public Animator healthBarFlareAnimator;
    public Sprite healthSprite;
    public Sprite maxHealthSprite;
    public TMP_Text healthBarText;

    public bool canEscapeObjectMenu = true;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }

        instance = this;

        _playerControls = new PlayerControls();
        mouseClick = _playerControls.Menus.MouseClick;
    }

    void Update()
    {
        PanelAlpha();
        Money();
    }

    //Barre de vie joueur 
    public void HurtPanels()
    {
        //launches hurt anim
        hurtPanel.SetTrigger("RedOut");
        //updates low hp panel scale
        float health = GameManager.instance.health;
        float maxHealth = GameManager.instance.maxHealth;
        if (health / maxHealth < 0.5f)
        {
            float panelScale = 1 + (3 * (health / maxHealth));
            lowHpPanel.localScale = new Vector3(panelScale, panelScale, 1);
        }
        else
        {
            lowHpPanel.localScale = new Vector3(4, 4, 1);
        }
    }

    public void HealthBar(float health)
    {
        //sets bar max and min
        healthBarText.text = (GameManager.instance.health * 10).ToString();
        //sets health bar length depending on the health remaining
        float initialFillPos = 916;
        float initialFillSize = 1833;
        float healthRatio = (float)health / (float)GameManager.instance.maxHealth;
        healthBarFill.localPosition = new Vector3(initialFillSize * (-1 + 1 * healthRatio), 0, 0);
        healthBarFill.sizeDelta = new Vector2(1 * healthRatio * initialFillSize, 145);
        healthBarFlare.position = healthBarFill.position + new Vector3(30, 0, 0);
        //plays flare
        healthBarFlareAnimator.CrossFade(Animator.StringToHash("Flare"), 0,0);
    }

    void Money()
    {
        //updates current money on screen
        MoneyUI.text = GameManager.instance.money.ToString();
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

    public void UpdateHUDIcons()
    {
        //used to reload data from the object when added
        for (int i = 0; i < 3; i++)
        {
            if (ObjectsManager.instance.itemObjectsInventory[i] != 999)
            {
                int id = ObjectsManager.instance.itemObjectsInventory[i];
                //update : box name, icon, description, rarity color
                Sprite icon = ObjectsManager.instance.objectSprites[id];
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
