using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectTextData", menuName = "ScriptableObjects/ObjectTextData", order = 1)]
public class ObjectTextData : ScriptableObject
{
    public string[] descriptions;
    public string[] names;
    public int[] itemCosts;
    public int[] rarity;
    public string[] rarityNames;
}
