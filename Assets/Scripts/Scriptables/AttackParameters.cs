using UnityEngine;
[CreateAssetMenu(fileName = "AttackParameters", menuName = "ScriptableObjects/AttackParameters")]

public class AttackParameters : ScriptableObject
{
    public float attackStartupLength;
    public float attackActiveLength;
    public float attackRecoverLength;
    
    public float pickStartupLength;
    public float pickActiveLength;
    public float pickRecoverLength;
    
    public float smashStartupLength;
    public float smashActiveLength;
    public float smashRecoverLength;
}
