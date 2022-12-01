using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatManager : MonoBehaviour
{
    public static CheatManager instance;

    private GameManager _gameManager;
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
        _uiManager = UIManager.instance;
        _objectsManager = ObjectsManager.instance;
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
                    case "summon" : 
                        switch (secondInput)
                        {
                            case "wanderer" :
                                GameObject wanderer = Instantiate(LevelManager.instance.basicEnemies[0]);
                                wanderer.transform.position = PlayerController.instance.gameObject.transform.position +
                                                              Vector3.up;
                                wanderer.GetComponent<Enemy>().StartCoroutine(wanderer.GetComponent<Enemy>().EnemyApparition());
                                break;
                            case "bull" :
                                GameObject bull = Instantiate(LevelManager.instance.basicEnemies[1]);
                                bull.transform.position = PlayerController.instance.gameObject.transform.position +
                                                          Vector3.up;
                                bull.GetComponent<Enemy>().StartCoroutine(bull.GetComponent<Enemy>().EnemyApparition());
                                break; 
                            case "shooter" :
                                GameObject shooter = Instantiate(LevelManager.instance.basicEnemies[2]);
                                shooter.transform.position = PlayerController.instance.gameObject.transform.position +
                                                             Vector3.up;
                                shooter.GetComponent<Enemy>().StartCoroutine(shooter.GetComponent<Enemy>().EnemyApparition());
                                break; 
                            case "mk" :
                                GameObject mk = Instantiate(LevelManager.instance.basicEnemies[3]);
                                mk.transform.position = PlayerController.instance.gameObject.transform.position +
                                                        Vector3.up;
                                mk.GetComponent<Enemy>().StartCoroutine(mk.GetComponent<Enemy>().EnemyApparition());
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
