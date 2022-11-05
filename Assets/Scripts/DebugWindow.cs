using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;

public class DebugWindow : MonoBehaviour
{
    //parameters visible
    private float _memoryAvailable;
    private float _memoryUsage;
    private float _cpuUsage;
    private float _gpuUsage;
    private float _currentFps;
    private float _timePassed;
    private Vector3 _playerVelocity;
    private Vector3 _cameraPos;
    
    private int _currentRoom;
    private int _enemiesAlive;
    private int _enemiesSpawned;
    private int _enemiesTotal;

    public TMP_Text debugText;

    private float _deltaTime;
    private PlayerController _pc;
    private GameObject _player;
    private Rigidbody _rb;
    private GameObject _cam;

    private bool _isActive;
    private string _activationKey;
    private GameObject _uiPanel;

    private void Awake()
    {
        _player = GameObject.Find("Player");
        _pc = _player.GetComponent<PlayerController>();
        _rb = _player.GetComponent<Rigidbody>();
        _cam = GameObject.Find("Camera");
        _uiPanel = transform.GetChild(0).gameObject;
    }

    //values
    void Start()
    {
        _activationKey = KeyCode.F1.ToString();
        _isActive = false;
        _uiPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!_isActive)
            {
                _isActive = true;
               _uiPanel.SetActive(true);
            }
            else
            {
                _isActive = false;
                _uiPanel.SetActive(false);
            }
        }

        if (_isActive)
        {
            _memoryAvailable = Profiler.GetMonoHeapSizeLong();
            _memoryUsage = Profiler.GetMonoUsedSizeLong();
        
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
            float fps = 1.0f / _deltaTime;
            _currentFps = Mathf.Ceil(fps);
            
            CreateTextCode();
        }
    }

    void CreateTextCode()
    {
        debugText.text = 
            "DebugWindow : " + " \n" + "\n" + //title
            "MemoryUsage : " + Mathf.Ceil(_memoryUsage/1000000) + "Mb /" + Mathf.Ceil(_memoryAvailable/1000000) + " ~" + Mathf.Ceil(_memoryUsage/_memoryAvailable) + "Mb \n" +
            "FPS : " + _currentFps + " \n" + " \n" +
            
            "Player Velocity : " + "(" + Mathf.Ceil(_rb.velocity.x) + " , " + Mathf.Ceil(_rb.velocity.z) + ")" + " \n" +
            "Camera Pos : " + _cam.transform.position + " \n" + "\n" +
            
            "Current Room : " + _currentRoom + "\n" +"\n" + 
            
            "Enemies Alive : " + _enemiesAlive +  "\n" +                       
            "Enemies Spawned : " + _enemiesSpawned + "\n" +                            
            "Total Enemies : " + _enemiesTotal + "\n"                             
            ;
    }
}
