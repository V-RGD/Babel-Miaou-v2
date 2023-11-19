using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AttackSystem : MonoBehaviour
{
    [Header("Events")] [SerializeField] protected UnityEvent onAttack;
    public Vector2 attackDir { get; protected set; }
    public bool isAttacking { get; protected set; }

    /// <summary>
    /// Base class used to quickly add attacks to an enemy
    /// </summary>
    [Serializable]
    public class Attack
    {
        public GameObject spriteParent;
        public GameObject collider;
        public SpriteRenderer[] sprites;
        public float warmup = 1;
        public float length = 1;
        public float endLag = 1;
        public UnityEvent onAttack;
    }

    /// <summary>
    /// Default coroutine for a melee attack
    /// </summary>
    /// <param name="attack">Attack parameters</param>
    /// <param name="anchor">Pivot for the collider rotation</param>
    /// <param name="dir">Direction of the attack</param>
    /// <param name="prepareColor">Color that warns the player that an attack is upcoming</param>
    /// <param name="attackColor">Color indicating that the attack is currently active and will damage player on contact</param>
    /// <returns></returns>
    protected IEnumerator DefaultMelee(Attack attack, Transform anchor, Vector2 dir, Color prepareColor, Color attackColor)
    {
        isAttacking = true;

        //rotates anchor around character
        //Vector2 correctedDir = Utilities.ConvertTo4Dir(dir);
        int rotation = 0;
        anchor.transform.rotation = Quaternion.Euler(0, 0, rotation);

        //enables visuals + colors attack for preparation
        attack.spriteParent.SetActive(true);
        ColorAttackSprites(attack.sprites, prepareColor);
        attack.collider.SetActive(false);

        yield return new WaitForSeconds(attack.warmup); //waits warmup

        //enables colliders and colors attack for activation
        ColorAttackSprites(attack.sprites, attackColor);
        attack.collider.SetActive(true);

        yield return new WaitForSeconds(attack.length); //waits length

        //disables both visuals and colliders
        attack.spriteParent.SetActive(false);
        attack.collider.SetActive(false);

        yield return new WaitForSeconds(attack.endLag); //waits end lag
        isAttacking = false; //stops attack
    }

    /// <summary>
    /// Default coroutine for a distance attack
    /// </summary>
    /// <param name="attack">Attack parameters</param>
    /// <param name="projectile">Transform of the projectile used for this attack</param>
    /// <param name="targetTile">Position taken by the projectile upon use</param>
    /// <param name="prepareColor">Color that warns the player that an attack is upcoming</param>
    /// <param name="attackColor">Color indicating that the attack is currently active and will damage player on contact</param>
    /// <returns></returns>
    protected IEnumerator DefaultDistance(Attack attack, Transform projectile, Vector2 targetTile, Color prepareColor,
        Color attackColor)
    {
        isAttacking = true;

        //places the projectile at the correct position
        projectile.position = targetTile;
        projectile.gameObject.SetActive(true); //activates warning / attacks
        //ColorAttackSprites(attack.sprites, EnemyColors.attackPrepare); //starts animation

        yield return new WaitForSeconds(attack.warmup); //waits warmup

        //ColorAttackSprites(attack.sprites, EnemyColors.attackActive);
        attack.collider.SetActive(true); //activates attack

        yield return new WaitForSeconds(attack.length); //waits attack length

        projectile.gameObject.SetActive(false); //disables warning / attacks

        yield return new WaitForSeconds(attack.endLag); //waits end lag

        isAttacking = false; //stops attack
    }

    /// <summary>
    /// Colors all the sprites defined in the inspector for this attack
    /// </summary>
    /// <param name="renderers">The sprite Renderers defined in the inspector</param>
    /// <param name="color">The color that will be applied to the renders</param>
    protected void ColorAttackSprites(SpriteRenderer[] renderers, Color color)
    {
        foreach (var sp in renderers)
        {
            sp.color = color;
        }
    }

    /// <summary>
    /// Used by parent classes to init their projectiles at start
    /// </summary>
    /// <param name="attackOutput">The attack that will use the projectile</param>
    /// <param name="prefab">The prefab of the projectile used</param>
    /// <param name="outVar">The transform variable that will hold the projectile instance</param>
    protected void InitProjectile(Attack attackOutput, Transform prefab, out Transform outVar)
    {
        //instantiate the specified prefab
        outVar = Instantiate(prefab);

        //associates collider and sprite parent
        attackOutput.collider = outVar.GetChild(0).gameObject;
        attackOutput.spriteParent = outVar.GetChild(1).gameObject;

        //connects sprites to enable recoloring
        int spritesAmount = attackOutput.spriteParent.transform.childCount;
        attackOutput.sprites = new SpriteRenderer[spritesAmount];
        for (int i = 0; i < spritesAmount; i++)
        {
            SpriteRenderer renderer = attackOutput.spriteParent.transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (renderer) attackOutput.sprites[i] = renderer;
        }

        //disables projectile
        outVar.gameObject.SetActive(false);
    }
}