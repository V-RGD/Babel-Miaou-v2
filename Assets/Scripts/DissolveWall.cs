using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DissolveWall : MonoBehaviour
{
    public GameObject cam;
    public float dissolveTimer;
    private Material objectMaterial;
    private bool _wallInBetween;
    private LayerMask _wallLayerMask;
    private GameObject objectToDissolve;

    private void Awake()
    {
        _wallLayerMask = LayerMask.NameToLayer("Wall");
        cam = GameObject.Find("Camera");
    }

    private void Update()
    {
        //GetComponent<MeshRenderer>().sharedMaterial.SetFloat("Dissolve", dissolveTimer);

        if (_wallInBetween)
        {
            dissolveTimer += Time.deltaTime;
        }
        else
        {
            dissolveTimer -= Time.deltaTime;
        }

        if (dissolveTimer > 1)
        {
            dissolveTimer = 1;
        }
        
        if (dissolveTimer < 0)
        {
            dissolveTimer = 0;
        }
        
        WallCheck();
    }
    
    void WallCheck()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, cam.transform.position - transform.position);
        Debug.DrawRay(transform.position, cam.transform.position - transform.position, Color.red);

        if (Physics.Raycast(ray, out hit, 1000, ~_wallLayerMask))
        {
            if (hit.collider.gameObject.CompareTag("Wall"))
            {
                _wallInBetween = true;
            }
            else
            {
                _wallInBetween = false;
            }
        }
    }
}


