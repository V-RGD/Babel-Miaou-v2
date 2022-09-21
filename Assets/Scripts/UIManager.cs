using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public TMP_Text MoneyUI;
    public TMP_Text HeartsUI;

    public Image panel;

    public bool doWhiteout;
    public bool doBlackout;

    public float transitionLenght;
    public float alpha;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        MoneyUI.text = gameManager.money.ToString();
        HeartsUI.text = gameManager.health.ToString();
        

        if (doWhiteout)
        {
            alpha -= Time.deltaTime / transitionLenght;
            panel.color = new Color(0, 0, 0, alpha);
        }

        if (doBlackout)
        {
            alpha += Time.deltaTime / transitionLenght;
            panel.color = new Color(0, 0, 0, alpha);
        }
    }
    
    public IEnumerator Whiteout()
    {
        alpha = 1;
        doWhiteout = true;
        yield return new WaitForSeconds(transitionLenght);
        panel.color = new Color(0, 0, 0, 0);
        doWhiteout = false;
    }
    
    public IEnumerator Blackout()
    {
        alpha = 0;
        doBlackout = true;
        yield return new WaitForSeconds(transitionLenght);
        panel.color = new Color(0, 0, 0, 1);
        doBlackout = false;
    }
}
