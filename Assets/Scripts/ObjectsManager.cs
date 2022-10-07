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
    
    public List<GameObject> itemObjectsInventory = new List<GameObject>(5);
    public GameObject[] itemList;
    public ObjectTextData itemDataScriptable;
    public Sprite[] objectSprites;

    //creates special pools
    [HideInInspector]public List<GameObject> shopPool;
    [HideInInspector]public List<GameObject> chestPool;
    [HideInInspector]public List<GameObject> specialChestPool;

    public GameObject[] uiItemBoxes;
    public GameObject objectMenu;

    public GameObject uiItemPrefab;

    private void Start()
    {
        AssignObjectInfos();
        CreateItemPools();
    }

    private void Update()
    {
        //ObjectsInInventory();
        UiItemBoxesUpdate();
        DisableObjectMenu();
    }

    void ObjectsInInventory()
    {
        //pour chaque case, si un objet est equippé (bool), on active l'id de l'objet.
        for (int i = 0; i < itemObjectsInventory.Count - 1; i++)
        {
            //check if an object is equipped
            if (itemObjectsInventory[i] != null)
            {
                //check the ID of the object to add additional effects
                switch (itemObjectsInventory[i].GetComponent<ItemDragDrop>().objectID)
                {
                    //activer les effets
                    
                }
            }
        }
    }
    void CreateItemPools()
    {
        //chest pool = reserved chest + commom
        foreach (var id in itemDataScriptable.chestReservedItems)
        {
            chestPool.Add(itemList[id]);
        }
        
        foreach (var id in itemDataScriptable.commonItems)
        {
            chestPool.Add(itemList[id]);
        }

        //special chest pool = special chest
        foreach (var id in itemDataScriptable.specialChestReservedItems)
        {
            specialChestPool.Add(itemList[id]);
        }
        
        //shop pool
        foreach (var id in itemDataScriptable.shopItemReservedItems)
        {
            shopPool.Add(itemList[id]);
        }
        foreach (var id in itemDataScriptable.commonItems)
        {
            shopPool.Add(itemList[id]);
        }
    }
    void AssignObjectInfos()
    {
        //assigns object info depending on it's position on the list.
        for (int i = 0; i < itemList.Length; i++)
        {
            itemList[i].GetComponent<Item>().objectID = i;
            itemList[i].GetComponent<Item>().description = itemDataScriptable.descriptions[i];
            itemList[i].GetComponent<Item>().itemName = itemDataScriptable.names[i];
            itemList[i].GetComponent<Item>().rarity = itemDataScriptable.rarity[i];
            switch (itemList[i].GetComponent<Item>().rarity)
            {
                case 1 : itemList[i].GetComponent<Item>().itemCost = 15; break;
                case 2 : itemList[i].GetComponent<Item>().itemCost = 25; break;
                case 3 : itemList[i].GetComponent<Item>().itemCost = 35; break;
                case 4 : itemList[i].GetComponent<Item>().itemCost = 35; break;
            }
            itemList[i].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = objectSprites[i];
        }
    }

    void UiItemBoxesUpdate()
    {
        //used to reload data from the object when added
        for (int i = 0; i < 6; i++)
        {
            if (itemObjectsInventory[i] != null)
            {
                int id = itemObjectsInventory[i].GetComponent<ItemDragDrop>().objectID;
                //update : box name, icon, description, rarity color
                string name = itemDataScriptable.names[id];
                Sprite icon = objectSprites[id];
                string desc = itemDataScriptable.descriptions[id];
                int rarity = itemDataScriptable.rarity[id];
                Color color = Color.grey;

                uiItemBoxes[i].transform.GetChild(1).GetComponent<TMP_Text>().text = name;
                //uiItemBoxes[i].transform.GetChild(3).GetComponent<Image>().sprite = icon;
                uiItemBoxes[i].transform.GetChild(2).GetComponent<TMP_Text>().text = desc;
                switch (rarity)
                {
                    case 1 : color = Color.green; break;
                    case 2 : color = Color.blue; break;
                    case 3 : color = Color.magenta; break;
                    case 4 : color = Color.yellow; break;
                }
                uiItemBoxes[i].transform.GetChild(0).GetComponent<Image>().color = color;
            }
            else
            {
                //shows empty box
                uiItemBoxes[i].transform.GetChild(1).GetComponent<TMP_Text>().text = "<Add Module>";
                //uiItemBoxes[i].transform.GetChild(4).GetComponent<Image>().sprite = null;
                uiItemBoxes[i].transform.GetChild(2).GetComponent<TMP_Text>().text = "";
                uiItemBoxes[i].transform.GetChild(0).GetComponent<Image>().color = Color.grey;
            }
        }
    }

    void DisableObjectMenu()
    {
        if (objectMenu.activeInHierarchy)
        {
            
        }
    }
}
