using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskConverter : MonoBehaviour
{
    //when a sprite mask is input, checks colors and indexes the pixel grid base on it
    [Header("Tile Indexes")] 
    [SerializeField] private int groundIndex = 1;
    [SerializeField] private int bridgeIndex = 2;
    [SerializeField] private int doorIndex = 3;
    [SerializeField] private int wallIndex = 4;
    [Header("Color ID")]
    [SerializeField] private Color groundColor = Color.green;
    [SerializeField] private Color wallColor = Color.blue;
    [SerializeField] private Color bridgeColor = Color.yellow;
    [SerializeField] private Color doorColor = Color.red;
    
    int[,] SpriteToMask
}
