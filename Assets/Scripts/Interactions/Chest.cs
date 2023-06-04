using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interactions
{
    public class Chest : MonoBehaviour
    {
        [Serializable] private class Loot
        {
            public float lootRate;
            public GameObject prefab;
        }

        [SerializeField] private List<Loot> possibleLoots;

        void OnOpen()
        {
            //builds a list with all the thresholds to compare when the draw will happen
            float itemThreshold = 0;
            float[] thresholds = new float[possibleLoots.Count];
            for (var i = 0; i < possibleLoots.Count; i++)
            {
                itemThreshold += possibleLoots[i].lootRate;
                thresholds[i+1] = itemThreshold;
            }

            if (itemThreshold > 1) Debug.LogError("The combined probabilities of the potential loots are superior to 1 ! This will lead to a rigged drop rate.");
            
            //draws a random number, and sees if it matches any threshold
            float roulette = Random.Range(0f, 1f);
            GameObject droppedLoot = possibleLoots[0].prefab;

            for (int i = 0; i < possibleLoots.Count; i++)
            {
                //if the roulette matches right between 2 thresholds, 
                if (roulette < thresholds[i] && roulette > thresholds[i+1])
                {
                    droppedLoot = possibleLoots[i].prefab;
                    break;
                }
            }

            //then finally loots item
            Instantiate(droppedLoot, transform.position, quaternion.identity);
        }
    }
}
