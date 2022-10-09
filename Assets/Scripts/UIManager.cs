using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    public PlayerControls playerControls;
    private InputAction escape;
    private InputAction mouseClick;
    private ObjectsManager _objectsManager;
    public TMP_Text MoneyUI;
    public GameObject[] heartUIs;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Image panel;

    [HideInInspector]public bool doWhiteout;
    [HideInInspector] public bool doBlackout;

    public float transitionLenght;
    private float _panelAlpha;

    public bool canEscapeObjectMenu = true;

    private void Awake()
    {
        playerControls = new PlayerControls();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        escape = playerControls.Menus.Escape;
        mouseClick = playerControls.Menus.MouseClick;
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
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

    public void ActiveObjectMenu()
    {
        if (!_objectsManager.objectMenu.activeInHierarchy)
        {
            //actives ui
            _objectsManager.objectMenu.SetActive(true);
            //stop time
            Time.timeScale = 0;
        }
    }
    

    public void DisableObjectMenu(InputAction.CallbackContext context)
    {
        if (_objectsManager.objectMenu.activeInHierarchy && canEscapeObjectMenu)
        {
            //si objet pas dans boite
            
            //retourne dans boite
            
            //deletes 6th box object
            Destroy(_objectsManager.itemObjectsInventory[5]);
            _objectsManager.itemObjectsInventory[5] = null;
            //disables menu
            _objectsManager.objectMenu.SetActive(false);
            //resume time
            Time.timeScale = 1;
        }
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
        escape.Enable();
        escape.performed += DisableObjectMenu;
        escape.ReadValue<bool>();
    }
    
    private void OnDisable()
    {
        mouseClick.Disable();
        escape.Disable();
    }
}
