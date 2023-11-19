using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PropsGenProfile", menuName = "ScriptableObjects/PropsGenProfile")]

public class PropsGenProfile : ScriptableObject
{
    [Serializable]public class Prop
    {
        public GameObject prefab;
        public float averageSize;
    }
    public List<Prop> outProps;
    public List<Prop> groundProps;
    public List<Prop> colliderProps;
}
