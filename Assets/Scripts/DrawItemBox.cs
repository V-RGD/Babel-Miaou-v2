using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawItemBox : MonoBehaviour
{
    public Item[] items = new Item[3];
    public DrawBoxVisuals[] boxVisuals = new DrawBoxVisuals[3];
    
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
        items[0].AccessToItemMenu();
        //disables menu
        _menuManager.drawMenu.gameObject.SetActive(false);
        //UpdateItemBox(1);
    }
    public void OnDrawBoxClick2()
    {
        //uses item as if it was collected normally
        items[1].AccessToItemMenu();
        //disables menu
        _menuManager.drawMenu.gameObject.SetActive(false);
        //UpdateItemBox(2);
    }
    public void OnDrawBoxClick3()
    {
        //uses item as if it was collected normally
        items[2].AccessToItemMenu();
        //disables menu
        _menuManager.drawMenu.gameObject.SetActive(false);
        //UpdateItemBox(3);
    }

    void UpdateItemBox(int item)
    {
        //assigns variables
        _menuManager.ObjectMenu();
        //instantiates a new ui item in the canvas
        GameObject newItem = Instantiate(_objectsManager.uiItemPrefab, _objectsManager.objectMenu.transform);
        _objectsManager.itemObjectsInventory[5] = newItem;
        //puts it in the 6th box
        _objectsManager.itemObjectsInventory[5].GetComponent<RectTransform>().transform.position = _objectsManager.uiItemBoxes[5].transform.position;
        //updates it's id
        _objectsManager.itemObjectsInventory[5].GetComponent<ItemDragDrop>().objectID = items[item].objectID;
        _objectsManager.itemObjectsInventory[5].GetComponent<Image>().sprite = _objectsManager.objectSprites[items[item].objectID];
        _objectsManager.itemObjectsInventory[5].GetComponent<ItemDragDrop>().boxAssociated = 5;
        _objectsManager.UiItemBoxesUpdate();
        Destroy(gameObject);
    }
}
