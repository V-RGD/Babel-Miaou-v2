using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject[] items;
    public List<Transform> spawnAreas;
    
    public 
    
    // Start is called before the first frame update
    void Start()
    {
        //choses 3 random items from a list of gameobjects
        //spawns them in 3 different transforms
        for (int i = 0; i < 3; i++)
        {
            Instantiate(items[Random.Range(0, items.Length)], spawnAreas[i]);
        }
        //if the player enters the object's area, has the right amount of cash, and presses the right button, items dissapear,
        //gives proper effect to the player, and removes money from player
        
    }
}
