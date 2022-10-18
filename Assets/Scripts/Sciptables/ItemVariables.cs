using UnityEngine;
[CreateAssetMenu(fileName = "ItemVariables", menuName = "ScriptableObjects/ItemVariables", order = 3)]

public class ItemVariables : ScriptableObject
{
    //variables managed by object manager

    //active effects accordingly
    //     case 0 : //boolean - player sees full map
    //         break;
    //     case 1 : //boolean - 10 kills - 1HP
    //         break;
    //     case 2 : //boolean - when entering a room, kills a random enemy in the room
    //         break;
    //     case 3 : //boolean - every 2 missing health, player gets 0.5 bonus attack
    //         break;
    //     case 4 : //boolean - when hit, gains 1.5x attack + -0,2s dexterity during 2s
    //         break;
    //     case 5 : //boolean - highlights important places on the map
    //         break;
    //     case 6 : //boolean - backstabbing an enemy increases damage
    //         break;
    //     case 7 : //boolean - killing an enemy increases damage by 1 during 1.5s. dash cooldown resets
    //         break;
    //     case 8 : //boolean - small attacks repels projectiles
    //         break;
    //     case 9 : //boolean - if full hp - small attacks launches projectiles with half base damage
    //         break;
    //     case 10 : //boolean - when hit : small shield appears , player invulnerable in the meantime
    //         break;
    //     case 11 : //boolean - charged attack deals 6x (not 5) damage and stuns enemies
    //         break;
    //     case 12 : //boolean - every damage is reduced by 1 (min 1)
    //         break;
    //     case 13 : //boolean - if enemy too close from player - bonus attack (cooldown 4s, does 2x base damage)
    //         break;
    //     case 14 : //boolean - if player hit, every enemy slowed down by 20%
    //         break;
    //     case 15 : //boolean - charged attack has 25% more range, can destroy certain obstacles
    //         break;
    //     case 16 : //boolean - reduces attack delay, if player attacks X times, next one is instant charged
    //         break;
    //     case 17 : //boolean - each HP superior than base health increases dmg by 0.1
    //         break;
    //     case 18 : //boolean - when player enters room, gets double damage, if hit, disables boost. gets boost again when entering next room
    //         break;
    //     case 19 : //boolean - can pay with eyes
    //         break;
    //     case 20 : //boolean - ball appears, if hit, launches in hit dir. Deals 2.0x damage
    //         break;
    //     case 21 : //boolean - little shit appears
    //         break;
    //     case 22 : //boolean - if dies, 50% chance to come back to life with 4 health (+anim)
    //         break;
    //     case 23 : //boolean - increases damage by 25%, boosts attack speed by 0.15s, increases HP max by 4, speed +1
    //         break;
    //     case 24 : //boolean - increases max HP by 4, projectiles have a 33% chance to dispawn.
    //         break;
    // }
}
