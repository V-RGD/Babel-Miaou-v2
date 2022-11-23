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

    void UpdateItemBox(int item)
    {
        // //assigns variables
        // _menuManager.ObjectMenu();
        // //puts it in the 6th box
        // _objectsManager.itemObjectsInventory[5] = item;
        // //updates visuals
        // _objectsManager.itemObjectsInventory[5].GetComponent<Image>().sprite = _objectsManager.objectSprites[items[item].objectID];
        // _objectsManager.itemObjectsInventory[5].GetComponent<ItemDragDrop>().boxAssociated = 5;
        // _objectsManager.UiItemBoxesUpdate();
        // Destroy(gameObject);
    }
    
    public void AccessToItemMenu(int id)
    {
        _menuManager.ObjectMenu();
        //puts it in the 6th box
        _objectsManager.itemObjectsInventory[3] = id;        //updates it's id
        //_objectsManager.uiItemBoxes[5].GetComponent<Image>().sprite = _objectsManager.objectSprites[id];
        _objectsManager.UiItemBoxesUpdate();
        //Destroy(gameObject);
    }
}
