using UnityEngine;

[CreateAssetMenu(fileName = "ObjectTextData", menuName = "ScriptableObjects/ObjectTextData", order = 1)]
public class ObjectTextData : ScriptableObject
{
    [Header("*Info Per Object*")]
    public string[] descriptions;
    public string[] names;
}
