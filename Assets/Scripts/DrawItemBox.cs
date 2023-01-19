using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DrawItemBox : MonoBehaviour
{
    public int[] items = new int[2];
    public DrawBoxVisuals[] boxVisuals = new DrawBoxVisuals[2];

    public bool isMenuActive;
    public int currentCursorPos;
    public RectTransform cursor;
    public float cursorOffsetDiff;
    private Vector3 _cursorStartPos;
    private PlayerControls _playerControls;
    private InputAction _cursorLeft;
    private InputAction _cursorRight;
    private InputAction _confirm;

    private void Awake()
    {
        _playerControls = new PlayerControls();
    }

    private void Start()
    {
        MenuManager.instance = GameObject.Find("UIManager").GetComponent<MenuManager>();
    }

    public void OnDrawBoxClick1()
    {
        //uses item as if it was collected normally
        AccessToItemMenu(items[0]);
        //disables menu
        MenuManager.instance.drawMenu.gameObject.SetActive(false);
        //UpdateItemBox(1);
    }
    public void OnDrawBoxClick2()
    {
        //uses item as if it was collected normally
        AccessToItemMenu(items[1]);
        //disables menu
        MenuManager.instance.drawMenu.gameObject.SetActive(false);
        //UpdateItemBox(2);
    }

    public void AccessToItemMenu(int id)
    {
        ObjectsManager.instance.uiItemBoxes[3].SetActive(true);
        MenuManager.instance.ObjectMenu();
        //puts it in the 6th box
        int newItem = id;
        ObjectsManager.instance.itemObjectsInventory[3] = newItem;
        //updates it's id
        ObjectsManager.instance.uiItemBoxes[3].transform.GetChild(3).GetComponent<Image>().sprite = ObjectsManager.instance.objectSprites[id];
        ObjectsManager.instance.UiItemBoxesUpdate();
        isMenuActive = true;
    }

    
    public void MoveCursor(int dir)
    {
        Debug.Log("moved cursor");
        int pos;
        //if it goes beyond max
        if (currentCursorPos + dir > 1)
        {
            pos = 0;
        }
        //if it goes under min
        else if (currentCursorPos + dir < 0)
        {
            pos = 1;
        }
        else //alright
        {
            pos = currentCursorPos + dir;
        }
        
        //moves box and indent
        cursor.transform.position = _cursorStartPos + Vector3.right * pos * cursorOffsetDiff;
        currentCursorPos = pos;
    }
    void MoveCursorLeft(InputAction.CallbackContext context)
    {
        if (isMenuActive)
        {
            MoveCursor(-1);
        }
    }
    void MoveCursorRight(InputAction.CallbackContext context)
    {
        if (isMenuActive)
        {
            MoveCursor(1);
        }
    }

    void ChooseItem(InputAction.CallbackContext context)
    {
        if (currentCursorPos == 0)
        {
            OnDrawBoxClick1();
            isMenuActive = false;
        }
        else
        {
            OnDrawBoxClick2();
            isMenuActive = false;
        }
    }

    private void OnEnable()
    {
        _cursorLeft = _playerControls.Menus.LeftArrow;
        _cursorRight = _playerControls.Menus.RightArrow;
        _confirm = _playerControls.Menus.Confirm;
        
        _cursorLeft.Enable();
        _cursorRight.Enable();
        _confirm.Enable();

        _cursorLeft.performed += MoveCursorLeft;
        _cursorRight.performed += MoveCursorRight;
        _confirm.performed += ChooseItem;
    }

    private void OnDisable()
    {
        _cursorLeft.Disable();
        _cursorRight.Disable();
        _confirm.Disable();
    }
}
