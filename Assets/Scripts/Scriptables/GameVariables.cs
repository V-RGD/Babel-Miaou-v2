using UnityEngine;
[CreateAssetMenu(fileName = "GameVariables", menuName = "ScriptableObjects/GameVariables")]

public class GameVariables : ScriptableObject
{
    //manages object and player variables before application

    //player base stats
    public float baseHealth;
    public float baseAttack;
    public float baseSpeed;
    public float baseDexterity;

    //objects
    public float sacredCrossLength;
    public float noHitSpeedRunDamageMultiplier;
    public GameObject eyeCollector;
    public int eyeCollectorCollectCeil;
    public float catLuckResRate;
    public float catLuckResHp;
    public float killingSpreeLength;
    public float killingSpreeDamage;
}
