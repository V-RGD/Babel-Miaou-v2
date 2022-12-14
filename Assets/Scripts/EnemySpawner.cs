using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    public enum EnemyType
    {
        Wanderer,
        Bull,
        Shooter,
        Tank,
        Marksman
    }
    public EnemyType enemyType;
}
