using UnityEngine;

[CreateAssetMenu(fileName = "ObjectTextData", menuName = "ScriptableObjects/ObjectTextData", order = 1)]
public class ObjectTextData : ScriptableObject
{
    [Header("*Info Per Object*")]
    public string[] descriptions;
    public string[] names;
    public int[] rarity;
    public string[] rarityNames;
    
    [Header("*Item Pools*")]
    public int[] shopItemReservedItems;
    public int[] chestReservedItems;
    public int[] commonItems;
    public int[] specialChestReservedItems;
}
