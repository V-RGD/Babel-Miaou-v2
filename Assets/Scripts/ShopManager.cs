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
        //3 bienfaits différents
        GameObject healMax = Instantiate(_objectsManager.healItem, spawnAreas[0].position + Vector3.up, quaternion.identity);
        GameObject healObject = Instantiate(_objectsManager.maxHealthItem, spawnAreas[1].position + Vector3.up, quaternion.identity);
        GameObject randomItem = Instantiate(_objectsManager.randomItem, spawnAreas[2].position + Vector3.up, quaternion.identity);
        healMax.GetComponent<Item>().isFromAShop = true;
        healObject.GetComponent<Item>().isFromAShop = true;
        randomItem.GetComponent<Item>().isFromAShop = true;
        randomItem.GetComponent<Item>().shopManager = this;
        healMax.transform.parent = spawnAreas[0].transform;
        healObject.transform.parent = spawnAreas[1].transform;
        randomItem.transform.parent = spawnAreas[2].transform;
    }
}
