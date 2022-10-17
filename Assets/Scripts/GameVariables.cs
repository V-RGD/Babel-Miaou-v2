using UnityEngine;

public class GameVariables : MonoBehaviour
{
    //manages object and player variables before application

    //player base stats
    [HideInInspector]public float baseHealth;
    [HideInInspector]public float baseAttack;
    [HideInInspector]public float baseDexterity;

    //objects
    public bool allSeeingEye; //actives full map
    public bool bloodBlade; //killing x enemies rewards y health
    public bool foreignFriend; //a random enemy is killed when entering a room.
    public bool glassCanon; //each xHP lost, gains yAtk;
    public bool catWrath; //if hit, attack multiplied by x, attack cooldown reduced by y, during z seconds
    public bool allKnowingEye; //highlights interesting places on the map
    public bool assassin; //backstabbing deals 1.5x dmg
    public bool killingSpree; //killing an enemy grants X extra damage during Y seconds. Dash is cooldown is reset
    
    public bool theForce; //small attacks repels projectiles
    public bool masterSword; //if full hp, small attacks launches projectiles dealing 50% damage
    public bool sacredCross; 
    public bool bluSmash;
    public bool innerPeace;
    public bool noPetting;
    public bool witherShield;
    
    public bool strongGrasp;
    public bool swiftArt;
    public bool tankPower;
    public bool noHit;
    public bool strangePact;
    
    public GameObject knittingBall;
    public GameObject eyeCollector;
    public bool catLuck;
    public bool catNip;
    public bool safetyBlessing;
    
    public float[] bloodBladeValues = {10, 1}; //killing x enemies rewards y health
    public float[] glassCanonValues = {2, 0.5f}; //each xHP lost, gains yAtk;
    public float[] catWrathValues = {1.5f, 0.2f, 2};
    
    public float masterSwordDamage;
    public int catLuckHpRes;
    public float[] catNipValues = { 0.25f, 0.15f, 4, 1 };

}
