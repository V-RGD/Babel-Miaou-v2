using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawItemBox : MonoBehaviour
{
    public int[] items = new int[2];
    public DrawBoxVisuals[] boxVisuals = new DrawBoxVisuals[2];
    
    private MenuManager _menuManager;
    private ObjectsManager _objectsManager;

    private void Start()
    {
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
        _menuManager = GameObject.Find("UIManager").GetComponent<MenuManager>();
    }

    public void OnDrawBoxClick1()
    {
        //uses item as if it was collected normally
        AccessToItemMenu(items[0]);
        //disables menu
        _menuManager.drawMenu.gameObject.SetActive(false);
        //UpdateItemBox(1);
    }
    public void OnDrawBoxClick2()
    {
        //uses item as if it was collected normally
        AccessToItemMenu(items[1]);
        //disables menu
        _menuManager.drawMenu.gameObject.SetActive(false);
        //UpdateItemBox(2);
    }

    public void AccessToItemMenu(int id)
    {
        _objectsManager.uiItemBoxes[3].SetActive(true);
        _menuManager.ObjectMenu();
        //puts it in the 6th box
        int newItem = id;
        _objectsManager.itemObjectsInventory[3] = newItem;
        //updates it's id
        _objectsManager.uiItemBoxes[3].transform.GetChild(3).GetComponent<Image>().sprite = _objectsManager.objectSprites[id];
        _objectsManager.UiItemBoxesUpdate();
    }
}
