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
    public float bsbEnemiesNeeded;
    public float bsbHealthReward;
    public float glassCanonHealthNeeded;
    public float glassCanonDamage;
    public float catWrathDamageMultiplier;
    public float catWrathDexterityIncrease;
    public float catWrathLength;
    public float assassinDamageMultiplier;
    public float killingSpreeDamage;
    public float killingSpreeLength;

    public float sacredCrossLength;
    public float bluSmashStunCounter;
    public float noPetCooldown;
    public float witherShieldSlowAmount;

    public float strongGraspRangeIncrease;
    public float swiftArtAttackDelayDecrease;
    public float strangePactRatio;
    public float tankPowerCeiling;
    public float tankPowerDamage;
    public float noHitSpeedRunDamageMultiplier;
    
    public GameObject knittingBall;
    public GameObject eyeCollector;
    public float eyeCollectorCollectCeil;
    public float catLuckResRate;
    public float catLuckResHp;
    public float catNipDamage;
    public float catNipDexIncrease;
    public float catNipHpIncrease;
    public float catNipSpeedIncrease;
    
    public float safetyBlessingRate;
}
