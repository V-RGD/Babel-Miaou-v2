using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOLCheck : MonoBehaviour
{
    public List<GameObject> sceneComponents;
    void Start()
    {
        for (int i = 0; i < sceneComponents.Count; i++)
        {
            if (LevelManager.instance.DDOL[i] == sceneComponents[i])
            {
                Destroy(LevelManager.instance.DDOL[i]);
            }
        }
    }
}
