using UnityEngine;

public class GameVariables : MonoBehaviour
{
    //manages object and player variables before application

    //player base stats
    [HideInInspector]public float baseHealth;
    [HideInInspector]public float baseAttack;
    [HideInInspector]public float baseDexterity;

    //objects
    public float[] bloodBladeValues = {10, 1}; //killing x enemies rewards y health
    public float[] glassCanonValues = {2, 0.5f}; //each xHP lost, gains yAtk;
    public float[] catWrathValues = {1.5f, 0.2f, 2};
    
    public float masterSwordDamage;
    public int catLuckHpRes;
    public float[] catNipValues = { 0.25f, 0.15f, 4, 1 };

    public float catWrathTime;
    public float sacredCrossTimer;
    public float glassCanonDamage;
    public float catWrath_attackCooldown;
    public float catWrath_damageMultiplier;
    public float killingSpreeTimer;
}
