using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipSprite : MonoBehaviour
{
    private float _flipCounter;
    private float _turnSpeed = 10;
    public GameObject sprite;
    void Update()
    {
        //Flip();
    }
    
    public void Flip()
    {
        Vector3 playerDir = PlayerController__old.instance.lastWalkedDir;
        if (playerDir.x > 0 && _flipCounter < 1)
        {
            _flipCounter += Time.deltaTime * _turnSpeed;
            sprite.transform.localScale = new Vector3(-_flipCounter, 1, 1);
        }
        if (playerDir.x < 0 && _flipCounter > -1)
        {
            _flipCounter -= Time.deltaTime * _turnSpeed;
            sprite.transform.localScale = new Vector3(-_flipCounter, 1, 1);
        }
    }
}
