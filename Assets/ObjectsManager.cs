using System;
using UnityEngine;

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
    
    public GameObject[] objectsEquipped;
    public GameObject[] objectsList;
    public ObjectTextData textData;
    public Sprite[] objectSprites;

    private void Start()
    {
        AssignObjectIDs();
    }

    void ObjectsInInventory()
    {
        for (int i = 0; i < objectsEquipped.Length; i++)
        {
            //check if an object is equipped
            if (objectsEquipped[i] != null)
            {
                //check the ID of the object to add additional effects
                switch (objectsEquipped[i].GetComponent<Item>().objectID)
                {
                    //acitver les effets
                    
                }
            }
        }
    }

    void AssignObjectIDs()
    {
        //assigns object id depending on it's position on the list.
        for (int i = 0; i < objectsList.Length; i++)
        {
            objectsList[i].GetComponent<Item>().objectID = i;
            objectsList[i].GetComponent<Item>().description = textData.descriptions[i];
            objectsList[i].GetComponent<Item>().itemName = textData.names[i];
            objectsList[i].GetComponent<Item>().itemCost = textData.itemCosts[i];
            objectsList[i].GetComponent<Item>().rarity = textData.rarity[i];
            objectsList[i].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = objectSprites[i];
        }
    }
}
