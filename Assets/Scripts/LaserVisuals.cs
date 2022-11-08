using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserVisuals : MonoBehaviour
{
    public Material _laserMat;
    private Vector3 _laserPoint;
    private Vector3 _laserDir;
    public bool isLaserOn;
    private bool _isLaserActive;
    private bool _isCharging;
    private float _laserTimer;

    private LineRenderer _lineRenderer;
    public FinalBossValues values;
    private GameManager _gameManager;
    private GameObject _player;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _player = GameObject.Find("Player");
        _laserMat = _lineRenderer.material;
    }

    private void Update()
    {
        VisualsUpdate();
    }

    void VisualsUpdate()
    {
        if (isLaserOn)
        {
            if (_isCharging)
            {
                //updates laser color
                if (_laserTimer < values.m_laserWarmup)
                {
                    _laserTimer += Time.deltaTime;
                }
                else
                {
                    _laserTimer = values.m_laserWarmup;
                }
                _laserMat.color = values.laserGradient.Evaluate(_laserTimer / values.m_laserWarmup);
                
                //updates laser position
                _laserDir = (_player.transform.position - transform.position).normalized;
                Vector3 hitPoint;
                RaycastHit hit;
            
                //check if a wall is in between laser
                if (Physics.Raycast(transform.position, _laserDir, out hit, 1000, values.groundLayerMask))
                {
                    hitPoint = hit.point;
                }
                else
                {
                    hitPoint = transform.position + _laserDir.normalized * 1000;
                }
            
                _lineRenderer.SetPosition(0, transform.position);
                _lineRenderer.SetPosition(1, hitPoint);
                
                //same as marksman laser but with a delay for each hand
                if (_isLaserActive)
                {
                    //shoots laser
                    _laserMat.color = Color.magenta;

                    //check if player touches laser
                    if (Physics.Raycast(transform.position, _laserDir, 4, values.playerLayerMask))
                    {
                        Debug.Log("hit player");
                        //deals damage
                        _gameManager.DealDamageToPlayer(values.m_laserDamage);
                        //can touch laser twice
                    }
                }
            }
        }
    }
    
    public IEnumerator ShootLaser()
    {
        Debug.Log("laser");
        isLaserOn = true;
        //while charging, laser is in direction of player, and color is updated depending on the current charge
        _lineRenderer.enabled = true;
        _isCharging = true;
        _laserTimer = 0;

        yield return new WaitForSeconds(values.m_laserWarmup);
        _isCharging = false;
        _laserMat.color = Color.cyan;
        //waits a bit for the player to avoid the laser
        yield return new WaitForSeconds(0.5f);
        //shoots laser
        _isLaserActive = true;
        //laser set inactive
        yield return new WaitForSeconds(values.m_laserLength);
        _isLaserActive = false;
        isLaserOn = false;
        _lineRenderer.enabled = false;
    }
    /*
    void M_Laser(LineRenderer laser)
    {
        
        //same as marksman laser but with a delay for each hand
        if (_isLaserOn)
        {
            //shoots laser
            laser.material.color = Color.magenta;

            //check if player touches laser
            if (Physics.Raycast(laser.gameObject.transform.position, m_laserDir, 4, values.playerLayerMask))
            {
                Debug.Log("hit player");
                //deals damage
                gameManager.DealDamageToPlayer(values.m_laserDamage);
                //can touch laser twice
            }
        }
        
        //updates laser color
        if (m_laserTimer < values.m_laserWarmup)
        {
            m_laserTimer += Time.deltaTime;
        }
        else
        {
            m_laserTimer = values.m_laserWarmup;
        }
        laser.material.color = values.laserGradient.Evaluate(m_laserTimer / values.m_laserWarmup);

        //updates laser position
        m_laserDir = (player.transform.position - laser.gameObject.transform.position).normalized;
        Vector3 hitPoint;
        RaycastHit hit;
            
        //check if a wall is in between laser
        if (Physics.Raycast(laser.gameObject.transform.position, m_laserDir, out hit, 1000, values.groundLayerMask))
        {
            hitPoint = hit.point;
        }
        else
        {
            hitPoint = laser.gameObject.transform.position + m_laserDir.normalized * 1000;
        }
            
        laser.SetPosition(0, laser.gameObject.transform.position);
        laser.SetPosition(1, hitPoint);
        
        if (_canAttack)
        {
            _canAttack = false;
            StartCoroutine(M_LaserAttack(laser));
        }
    }

    #endregion*/
}
