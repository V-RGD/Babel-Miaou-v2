using UnityEngine;
[CreateAssetMenu(fileName = "EnemyType", menuName = "ScriptableObjects/EnemyType", order = 2)]
public class EnemyType : ScriptableObject
{
    //tweak values
    [Header("Enemy Stats")]
    public float speed;
    public int maxHealth;
    public int damage;
    public float attackRange;
    public float attackCooldown;
    public int eyesDropped;

    //values
    [Header("Special Stats")]
    public int dashForce;
    public int projectileDamage;
    public int projectileForce;
    public float stunLenght;
    public float shootWarmup;
    public float dashWarmUp;
    public float recoilForce;
    public float attackForce;
    public float maxSpeed;
    public float bumpForce;
    
    //components
    [Header("*Objects*")]
    public GameObject eyeToken;
    public GameObject mageProjectile;
}
