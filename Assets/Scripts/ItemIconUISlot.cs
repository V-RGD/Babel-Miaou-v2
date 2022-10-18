using UnityEngine;
using UnityEngine.EventSystems;

public class ItemIconUISlot : MonoBehaviour , IDropHandler
{
    private ObjectsManager _objectsManager;
    public int boxIndex;
    private void Start()
    {
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            //updates new item equipped
            if (_objectsManager.itemObjectsInventory[boxIndex] == null)
            {
                //equips item
                eventData.pointerDrag.GetComponent<RectTransform>().transform.position = GetComponent<RectTransform>().transform.position;
                _objectsManager.itemObjectsInventory[boxIndex] = eventData.pointerDrag.gameObject;
                eventData.pointerDrag.GetComponent<ItemDragDrop>().boxAssociated = boxIndex;
                if (boxIndex != 5)
                {
                    _objectsManager.OnObjectEquip(eventData.pointerDrag.gameObject);
                }
            }
            else
            {
                //switches item
                GameObject switchingObject = _objectsManager.itemObjectsInventory[boxIndex];
                int switchingBoxIndex = eventData.pointerDrag.GetComponent<ItemDragDrop>().boxAssociated;

                //equips new one
                eventData.pointerDrag.GetComponent<RectTransform>().transform.position = GetComponent<RectTransform>().transform.position;
                _objectsManager.itemObjectsInventory[boxIndex] = eventData.pointerDrag.gameObject;
                eventData.pointerDrag.GetComponent<ItemDragDrop>().boxAssociated = boxIndex;
                
                //switching item goes to first object location
                switchingObject.GetComponent<RectTransform>().transform.position = _objectsManager.uiItemBoxes[switchingBoxIndex].GetComponent<RectTransform>().transform.position;
                _objectsManager.itemObjectsInventory[switchingBoxIndex] = eventData.pointerDrag.gameObject;
                eventData.pointerDrag.GetComponent<ItemDragDrop>().boxAssociated = switchingBoxIndex;
            }
        }
    }
}
