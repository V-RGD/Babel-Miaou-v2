using UnityEngine;
[CreateAssetMenu(fileName = "AttackParameters", menuName = "ScriptableObjects/AttackParameters")]

public class AttackParameters : ScriptableObject
{
    public float attackStartupLength;
    public float attackActiveLength;
    public float attackRecoverLength;
    
    public float spinStartupLength = 0.2f;
    public float spinActiveLength = 0.8f;
    public float spinRecoverLength = 0.1f;
    
    public float smashStartupLength;
    public float smashChargeLength = 1;
    public float smashActiveLength;
    public float smashRecoverLength;
}
