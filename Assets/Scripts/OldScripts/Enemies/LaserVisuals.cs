using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
    private GameObject _player;

    private ParticleSystem _chargeFx;
    private ParticleSystem _laserFx;
    private AudioSource _source;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _player = GameObject.Find("Player");
        _laserMat = _lineRenderer.material;

        _chargeFx = transform.GetChild(1).GetComponent<ParticleSystem>();
        _laserFx = transform.GetChild(0).GetComponent<ParticleSystem>();
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
                if (_laserTimer < FinalBossIA.instance.values.m_laserWarmup)
                {
                    _laserTimer += Time.deltaTime;
                }
                else
                {
                    _laserTimer = FinalBossIA.instance.values.m_laserWarmup;
                }
                _laserMat.color = FinalBossIA.instance.values.laserGradient.Evaluate(_laserTimer / FinalBossIA.instance.values.m_laserWarmup);
                
                //updates laser position
                _laserDir = (_player.transform.position - transform.position).normalized;
                Vector3 hitPoint;
                RaycastHit hit;
            
                //check if a wall is in between laser
                if (Physics.Raycast(transform.position, _laserDir, out hit, 1000, FinalBossIA.instance.values.groundLayerMask))
                {
                    hitPoint = hit.point;
                }
                else
                {
                    hitPoint = transform.position + _laserDir.normalized * 1000;
                }
            
                _lineRenderer.SetPosition(0, transform.position);
                _lineRenderer.SetPosition(1, hitPoint);
            }
            //same as marksman laser but with a delay for each hand
            if (_isLaserActive)
            {
                //shoots laser
                _lineRenderer.enabled = false;
                //check if player touches laser
                if (Physics.Raycast(transform.position, _laserDir, 4000, FinalBossIA.instance.values.playerLayerMask))
                {
                    //deals damage
                    GameManager_old.instance.DealDamageToPlayer(FinalBossIA.instance.values.m_laserDamage);
                    //can touch laser twice
                }
            }
        }
    }
    
    public IEnumerator ShootLaser()
    {
        isLaserOn = true;
        //while charging, laser is in direction of player, and color is updated depending on the current charge
        _lineRenderer.enabled = true;
        _isCharging = true;
        _laserTimer = 0;
        _chargeFx.Play();
        yield return new WaitForSeconds(FinalBossIA.instance.values.m_laserWarmup);
        _chargeFx.Stop();
        _laserFx.Play();
        _source.PlayOneShot(GameSounds.instance.marksmanShoot[0]);
        _laserFx.gameObject.transform.LookAt(_player.transform);
        _isCharging = false;
        //waits a bit for the player to avoid the laser
        yield return new WaitForSeconds(0.4f);
        //shoots laser
        _isLaserActive = true;
        //laser set inactive
        yield return new WaitForSeconds(FinalBossIA.instance.values.m_laserLength);
        _isLaserActive = false;
        _laserFx.Stop();
        isLaserOn = false;
        _lineRenderer.enabled = false;
    }
}
