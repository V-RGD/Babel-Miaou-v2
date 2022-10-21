using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour
{
    public List<GameObject> items;
    public List<Transform> spawnAreas;
    private ObjectsManager _objectsManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
        items = _objectsManager.shopPool;
        
        for (int i = 0; i < 3; i++)
        {
            spawnAreas.Add(transform.GetChild(i).transform);
        }
        
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
