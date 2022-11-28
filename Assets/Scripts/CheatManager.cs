using System;
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

    public void OpenCommandLine()
    {
        //freeze game
        Time.timeScale = 0;
        inputPanel.SetActive(true);
        //let player type
        canEnterCommand = true;
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
            string firstInput = String.Empty;
            string secondInput = String.Empty;
            string thirdInput = String.Empty;
            if (split[0] != null)
            {
                firstInput = split[0];
            }
            if (split[1] != null)
            {
                secondInput = split[1];
            }
            if (split[2] != null)
            {
                thirdInput = split[2];
            }

            if (firstInput != null)
            {
                //first range
                switch (firstInput)
                {
                    case "object" :
                        //if object, asks for desired box
                        int box = Convert.ToInt32(secondInput);
                        //then asks for desired object
                        int item = Convert.ToInt32(thirdInput);
                        //then adds item to inventory
                        _objectsManager.ReplaceItem(box, item);
                        break;
                    
                    case "health" : 
                        int healAmount = Convert.ToInt32(secondInput); 
                        _gameManager.health += healAmount;
                        //increases cap
                        if (_gameManager.health > _gameManager.maxHealth)
                        { 
                            _gameManager.maxHealth = _gameManager.health;
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
                                SceneManager.LoadScene("BossRoom");
                                break;
                            case "room" :
                                int room = Convert.ToInt32(thirdInput);
                                PlayerController.instance.gameObject.transform.position = 
                                LevelManager.instance.roomList[room].transform.position;
                            break; 
                        } 
                        break;
                }
            }
            
            Debug.Log("invalid command");
            CloseCommandLine();
        }
    }
}
