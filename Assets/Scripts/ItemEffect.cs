using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    public bool activeEffect;
    public bool isEquipped;
    public int objectID;
    public int rarity;
    public string objectName;

    private ObjectsManager _objectsManager;

    private void Start()
    {
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
    }

    void ActiveEffect()
    {
        switch (objectID)
        {
            // 1 to 9 : bonuses
            
            //10 to 30 : equipements
        }
    }
}
