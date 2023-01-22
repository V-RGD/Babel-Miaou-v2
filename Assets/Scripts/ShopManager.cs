using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public class ShopManager : MonoBehaviour
{
    public List<int> itemsToChooseFrom;
    public List<Transform> spawnAreas;
    private ObjectsManager _objectsManager;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _objectsManager = ObjectsManager.instance;
        itemsToChooseFrom = _objectsManager.itemList;
        
        for (int i = 0; i < 3; i++)
        {
            spawnAreas.Add(transform.GetChild(i).transform);
        }
        //de la vie max (remplit aussi la vie)
        //du soin (en plus grande quantité)
        //2 bienfaits différents
        GameObject healMax = Instantiate(_objectsManager.healItem, spawnAreas[0].position + Vector3.up * 5, Quaternion.Euler(0, -45, 0));
        GameObject healObject = Instantiate(_objectsManager.maxHealthItem, spawnAreas[1].position + Vector3.up * 5, Quaternion.Euler(0, -45, 0));
        GameObject randomItem = Instantiate(_objectsManager.randomItem, spawnAreas[2].position + Vector3.up * 5, Quaternion.Euler(0, -45, 0));
        healMax.GetComponent<Item>().isFromAShop = true;
        healObject.GetComponent<Item>().isFromAShop = true;
        randomItem.GetComponent<Item>().isFromAShop = true;
        randomItem.GetComponent<Item>().shopManager = this;
        healMax.transform.parent = transform;
        healObject.transform.parent = transform;
        randomItem.transform.parent = transform;
        healMax.SetActive(true);
        healObject.SetActive(true);
        randomItem.SetActive(true);
    }
}
