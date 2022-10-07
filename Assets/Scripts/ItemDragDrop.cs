using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public int objectID;
    public int boxAssociated;
    public ObjectsManager _objectsManager;
    public UIManager uiManager;

    private void Awake()
    {
        canvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //removes old assignation
        _objectsManager.itemObjectsInventory[eventData.pointerDrag.GetComponent<ItemDragDrop>().boxAssociated] = null;
        Debug.Log("dragging");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        if (_objectsManager.itemObjectsInventory[boxAssociated] == null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().transform.position = _objectsManager.uiItemBoxes[boxAssociated].GetComponent<RectTransform>().transform.position;
            _objectsManager.itemObjectsInventory[boxAssociated] = eventData.pointerDrag.gameObject;
            eventData.pointerDrag.GetComponent<ItemDragDrop>().boxAssociated = boxAssociated;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}