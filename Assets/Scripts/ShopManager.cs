using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour
{
    public List<GameObject> itemsToChooseFrom;
    public List<Transform> spawnAreas;
    private ObjectsManager _objectsManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
        itemsToChooseFrom = _objectsManager.shopPool;
        
        for (int i = 0; i < 3; i++)
        {
            spawnAreas.Add(transform.GetChild(i).transform);
        }
        
        //de la vie max (remplit aussi la vie)
        GameObject healMax = Instantiate(_objectsManager.healToken, spawnAreas[0].position, quaternion.identity);
        healMax.GetComponent<Item>().isFromAShop = true;
        //du soin (en plus grande quantité)
        GameObject healObject = Instantiate(_objectsManager.healToken, spawnAreas[1].position, quaternion.identity);
        healObject.GetComponent<Item>().isFromAShop = true;
        //3 bienfaits différents
        //Instantiate(_objectsManager.randomItem, spawnAreas[2].position, quaternion.identity);
        
        //choses 3 random items from a list of gameobjects
        //spawns them in 3 different transforms
        for (int i = 0; i < 3; i++)
        {
            //GameObject item = Instantiate(items[Random.Range(0, items.Count)], spawnAreas[i].position + Vector3.up * 2, quaternion.identity, spawnAreas[i]);
            //item.GetComponent<Item>().isFromAShop = true;
            //items.Remove(item);
        }
        //if the player enters the object's area, has the right amount of cash, and presses the right button, items dissapear,
        //gives proper effect to the player, and removes money from player
    }
}
