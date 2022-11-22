using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EyeChain : MonoBehaviour
{
    private List<EyeChain> connectedToMe = new List<EyeChain>();
    [HideInInspector]public bool isbase;
    private bool connectedToBase;
    private float range = 20;
    [HideInInspector]public FinalBossIA ia;
    private LayerMask playerLayerMask;
    public bool isExternal;
    public GameObject laserVisual;

    private void Start()
    {
        playerLayerMask = LayerMask.GetMask("Player");
        if (isExternal)
        {
            range = 0;
        }
    }

    public IEnumerator CheckConnection()
    {
        //the base is automatically connected to the web
        if (isbase)
        {
            connectedToBase = true;
        }
        
        //connects to any eye close to it
        foreach (var eye in ia.eyeList)
        {
            if ((eye.transform.position - transform.position).magnitude < range)
            {
                connectedToMe.Add(eye.GetComponent<EyeChain>());
            }
        }

        //check if any of the eyes it's connected to, is connected
        foreach (var eye in connectedToMe)
        {
            if (eye.connectedToBase)
            {
                connectedToBase = true;
            }
        }
        
        //if is still not connected, connects to nearest eye
        if (!connectedToBase)
        {
            float smallestDist = 10000;
            int closestEye = 0;
            //takes the nearest distance from me
            for (int i = 0; i < ia.eyeList.Count; i++)
            {
                float distanceFromMe = (ia.eyeList[i].transform.position - transform.position).magnitude;
                if (distanceFromMe < smallestDist)
                {
                    smallestDist = distanceFromMe;
                    closestEye = i;
                }
            }
            //connects it
            connectedToMe.Add(ia.eyeList[closestEye].GetComponent<EyeChain>());
        }
        
        //---draws web
        List<GameObject> lrList = new List<GameObject>();
        foreach (var eye in connectedToMe)
        {
            GameObject laserFx = Instantiate(laserVisual, transform.position, Quaternion.identity);
            laserFx.transform.LookAt(eye.transform.position);
            laserFx.GetComponent<VisualEffect>().Stop();
            laserFx.transform.localScale = new Vector3(0.25f, .25f, 0.25f) * ((eye.transform.position - transform.position).magnitude / 10);
            lrList.Add(laserFx);
        }
        
        //waits for 2 sec
        yield return new WaitForSeconds(2);
        
        //---activates web ---checks if the player is going through connections -- activates vfx
        foreach (var laser in lrList)
        {
            laser.GetComponent<VisualEffect>().Stop();
            laser.GetComponent<VisualEffect>().Play();
        }
        yield return new WaitForSeconds(2);

        //resets all values for next attack
        foreach (var line in lrList)
        {
            Destroy(line);
        }
        foreach (var eye in ia.eyeList)
        {
            eye.SetActive(false);
        }
        lrList.Clear();
        connectedToMe.Clear();
        connectedToBase = false;
        isbase = false;
    }
}
