using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public TMP_Text MoneyUI;
    public TMP_Text HeartsUI;

    
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        MoneyUI.text = gameManager.money.ToString();
        HeartsUI.text = gameManager.health.ToString();
    }

    
}
