using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsManager : MonoBehaviour
{
    //values that can be altered by items
    
    //private bool All_Seeing_Eye;
    //private bool Blood_Sucking_Blade;
    //private bool Scorch_Blade;
    //private bool Wither_Shield;
    //private bool Foreign_Friend;
    private bool Glass_Canon;   //moins de vie, + de dmg
    private bool Wrath_of_the_Cat;  //prends des dmg : boost de dmg - vitesse - dex
    //private bool All_Knowing_Eye;
    //private bool Doppelganger;
    private bool The_Assasin;   //2x dmg si dans le dos
    private bool Killing_Spree;  //si ennemi tué, boost dmg et dash timer = 0
    //private bool Eye_Collector;

    //private bool Safety_Blessing;
    //private bool May_the_Force;
    //private bool Sword_of_the_Master;
    //private bool Sacred_Cross;
    private bool Bludgeoning_Smash; //attaque de zone inflige dmg + et etourdit
    private bool Inner_Peace; //reduit les degats subis
    //private bool Knitting_Ball;
    //private bool No_Petting_Today;

    private bool Cat_Luck; //50% de chance de ressuciter, est détruit après usage
    //private bool Strong_Grasp;
    private bool Swiftness_Art; //augmente dextérité, 4eme attaque = atk chargée
    private bool Catnip; //augmente toutes les stats
    private bool Power_of_Tanking; //plus le joueur a de vies en plus, plus il est puissant
    //private bool Grandfather_Gift;
    private bool No_Hit_Speedrun; //rentre dans une salle, dgt doublés, annule jusqu'a la prochaine salle si prend des degats
    private bool Strange_Pact; //peut payer en vie si pas assez d'yeux (1 = X)
    
    public List<GameObject> objectsEquipped = new List<GameObject>(5);
    public GameObject[] objectsList;
    public ObjectTextData textData;
    public Sprite[] objectSprites;

    //creates special pools
    [HideInInspector]public List<GameObject> shopPool;
    [HideInInspector]public List<GameObject> chestPool;
    [HideInInspector]public List<GameObject> specialChestPool;

    public GameObject[] itemBoxes;

    private void Start()
    {
        CreateItemPools();
        AssignObjectInfos();
    }

    private void Update()
    {
        ObjectsInInventory();
    }

    void ObjectsInInventory()
    {
        //pour chaque case, si un objet est equippé (bool), on active l'id de l'objet.
        ItemBoxesUpdate();
        for (int i = 0; i < objectsEquipped.Count; i++)
        {
            //check if an object is equipped
            if (objectsEquipped[i] != null)
            {
                //check the ID of the object to add additional effects
                switch (objectsEquipped[i].GetComponent<Item>().objectID)
                {
                    //activer les effets
                    
                }
            }
        }
    }
    void CreateItemPools()
    {
        //chest pool = reserved chest + commom
        foreach (var id in textData.chestReservedItems)
        {
            chestPool.Add(objectsList[id]);
        }
        
        foreach (var id in textData.commonItems)
        {
            chestPool.Add(objectsList[id]);
        }

        //special chest pool = special chest
        foreach (var id in textData.specialChestReservedItems)
        {
            specialChestPool.Add(objectsList[id]);
        }
        
        //shop pool
        foreach (var id in textData.shopItemReservedItems)
        {
            shopPool.Add(objectsList[id]);
        }
        foreach (var id in textData.commonItems)
        {
            shopPool.Add(objectsList[id]);
        }
    }
    void AssignObjectInfos()
    {
        //assigns object id depending on it's position on the list.
        for (int i = 0; i < objectsList.Length; i++)
        {
            objectsList[i].GetComponent<Item>().objectID = i;
            objectsList[i].GetComponent<Item>().description = textData.descriptions[i];
            objectsList[i].GetComponent<Item>().itemName = textData.names[i];
            objectsList[i].GetComponent<Item>().rarity = textData.rarity[i];
            switch (objectsList[i].GetComponent<Item>().rarity)
            {
                case 1 : objectsList[i].GetComponent<Item>().itemCost = 15; break;
                case 2 : objectsList[i].GetComponent<Item>().itemCost = 25; break;
                case 3 : objectsList[i].GetComponent<Item>().itemCost = 35; break;
                case 4 : objectsList[i].GetComponent<Item>().itemCost = 35; break;
            }
            objectsList[i].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = objectSprites[i];
        }
    }

    void ItemBoxesUpdate()
    {
        for (int i = 0; i < itemBoxes.Length; i++)
        {
            if (objectsEquipped[i] != null)
            {
                int id = objectsEquipped[i].GetComponent<Item>().objectID;
                //update : box name, icon, description, rarity color
                string name = textData.names[id];
                Sprite icon = objectSprites[id];
                string desc = textData.descriptions[id];
                int rarity = textData.rarity[id];
                Color color = Color.grey;

                itemBoxes[i].transform.GetChild(1).GetComponent<TMP_Text>().text = name;
                itemBoxes[i].transform.GetChild(3).GetComponent<Image>().sprite = icon;
                itemBoxes[i].transform.GetChild(2).GetComponent<TMP_Text>().text = desc;
                switch (rarity)
                {
                    case 1 : color = Color.green; break;
                    case 2 : color = Color.blue; break;
                    case 3 : color = Color.magenta; break;
                    case 4 : color = Color.yellow; break;
                }
                itemBoxes[i].transform.GetChild(0).GetComponent<Image>().color = color;
            }
            else
            {
                //shows empty box
                itemBoxes[i].transform.GetChild(1).GetComponent<TMP_Text>().text = "<Add Module>";
                itemBoxes[i].transform.GetChild(3).GetComponent<Image>().sprite = null;
                itemBoxes[i].transform.GetChild(2).GetComponent<TMP_Text>().text = "";
                itemBoxes[i].transform.GetChild(0).GetComponent<Image>().color = Color.grey;
            }
        }
    }
}
