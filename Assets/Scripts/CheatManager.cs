using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatManager : MonoBehaviour
{
    public static CheatManager instance;

    private GameManager _gameManager;
    private LevelManager _lm;
    private GameScore _gameScore;
    private ObjectsManager _objectsManager;
    private UIManager _uiManager;
    public TMP_InputField field;
    public GameObject inputPanel;
    public bool canEnterCommand;
    private string _consoleCommand;
    private string previousCommand;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
    }

    private void Start()
    {
        _gameManager = GameManager.instance;
        _gameScore = GameScore.instance;
        _uiManager = UIManager.instance;
        _objectsManager = ObjectsManager.instance;
        _lm = LevelManager.instance;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (previousCommand != String.Empty)
            {
                field.text = previousCommand;
            }
        }
    }

    public void OpenCommandLine()
    {
        //freeze game
        Time.timeScale = 0;
        inputPanel.SetActive(true);
        //let player type
        canEnterCommand = true;
        field.Select();
    }
    
    public void CloseCommandLine()
    {
        //freeze game
        Time.timeScale = 1;
        inputPanel.SetActive(false);
        //let player type
        canEnterCommand = false;
    }

    public void TypeCommand()
    {
        if (canEnterCommand)
        {
            string command = field.text;
            string[] split = command.Split(' ');
            List<string> inputs = new List<string>();
            for (int i = 0; i < split.Length; i++)
            {
                inputs.Add(split[i]);
            }
            //security
            inputs.Add(String.Empty);
            inputs.Add(String.Empty);
            inputs.Add(String.Empty);
            string firstInput = String.Empty;
            string secondInput = String.Empty;
            string thirdInput = String.Empty;
            if (inputs[0] != String.Empty)
            {
                firstInput = split[0];
            }
            if (inputs[1] != String.Empty)
            {
                secondInput = split[1];
            }
            if (inputs[2] != String.Empty)
            {
                thirdInput = split[2];
            }

            if (firstInput != String.Empty)
            {
                //first range
                switch (firstInput)
                {
                    case "object" :
                        //if object, asks for desired box
                        int box = Convert.ToInt32(secondInput);
                        //then asks for desired object
                        string item = thirdInput;
                        //then adds item to inventory
                        if (box <= _objectsManager.itemObjectsInventory.Count - 1)
                        {
                            switch (item)
                            {
                                case "KillingSpree" : _objectsManager.ReplaceItem(box, 0);
                                    break;
                                case "SacredCross" : _objectsManager.ReplaceItem(box, 1);
                                    break;
                                case "StinkyFish" : _objectsManager.ReplaceItem(box, 2);
                                    break;
                                case "EyeCollector" : _objectsManager.ReplaceItem(box, 3);
                                    break;
                                case "CatLuck" : _objectsManager.ReplaceItem(box, 4);
                                    break;
                                case "EarthQuake" : _objectsManager.ReplaceItem(box, 5);
                                    break;
                                case "NoHit" : _objectsManager.ReplaceItem(box, 6);
                                    break;
                            }
                        }
                        break;
                    
                    case "health" : 
                        int healAmount = Convert.ToInt32(secondInput); 
                        _gameManager.health += healAmount;
                        //increases cap
                        if (_gameManager.health > _gameManager.maxHealth)
                        { 
                            //_gameManager.maxHealth = _gameManager.health;
                        }
                        _uiManager.HealthBar(_gameManager.health);
                        break;
                    
                    case "damage" :
                        int damage = Convert.ToInt32(secondInput);
                        PlayerAttacks.instance.attackStat += damage;
                        break;
                    
                    case "money" : 
                        int money = Convert.ToInt32(secondInput);
                        _gameManager.money += money;
                        break;
                    
                    case "tp" : 
                        switch (secondInput)
                        {
                            case "boss" :
                                SceneManager.LoadScene("BossScene");
                                break;
                            case "room" :
                                int room = Convert.ToInt32(thirdInput);
                                if (room <= LevelManager.instance.roomList.Count)
                                {
                                    PlayerController.instance.gameObject.transform.position = 
                                        LevelManager.instance.roomList[room].transform.position;
                                }
                                break; 
                        } 
                        break;
                    
                    case "load" : 
                        switch (secondInput)
                        {
                            case "main" :
                                SceneManager.LoadScene("MainScene");
                                break;
                            case "boss" :
                                SceneManager.LoadScene("BossScene");
                                break; 
                            case "menu" :
                                SceneManager.LoadScene("MainMenu");
                                break; 
                        } 
                        break;
                    case "spawn" :
                        GameObject enemySpawning = LevelManager.instance.basicEnemies[0];
                        int enemyType = 0;
                        switch (secondInput)
                        {
                            case "wanderer" :
                                enemySpawning = Instantiate(LevelManager.instance.basicEnemies[0]);
                                enemyType = 0;
                                break;
                            case "bull" :
                                enemySpawning = Instantiate(LevelManager.instance.basicEnemies[1]);
                                enemyType = 1;
                                break; 
                            case "shooter" :
                                enemySpawning = Instantiate(LevelManager.instance.basicEnemies[2]);
                                enemyType = 2;
                                break; 
                            case "tank" :
                                enemySpawning = Instantiate(LevelManager.instance.basicEnemies[3]);
                                enemyType = 3;
                                break;
                            case "mk" :
                                enemySpawning = Instantiate(LevelManager.instance.basicEnemies[4]);
                                enemyType = 4;
                                break;
                        }
                        enemySpawning.transform.position = PlayerController.instance.gameObject.transform.position -
                                                           Vector3.up * 0.9f;
                        //spawns enemy
                        Enemy component = enemySpawning.GetComponent<Enemy>();
                        component.room = gameObject;
                        component.StartCoroutine(component.EnemyApparition());
                        
                        //sets variables
                        component.health = _lm.matrices[enemyType].enemyValues[1].x;
                        component.damage = _lm.matrices[enemyType].enemyValues[1].y;
                        component.speed = _lm.matrices[enemyType].enemyValues[1].z;
                        component.eyesLooted = _lm.matrices[enemyType].enemyValues[1].w;
                        break;
                    case "score" : 
                        switch (secondInput)
                        {
                            case "set" :
                                _gameScore.SetPlayerScore(Convert.ToInt32(thirdInput));
                                break;
                            case "update" :
                                _gameScore.UpdateBoard();
                                break;
                        } 
                        break;
                }
            }

            previousCommand = field.text;
            field.text = String.Empty;
            CloseCommandLine();
        }
    }
}
