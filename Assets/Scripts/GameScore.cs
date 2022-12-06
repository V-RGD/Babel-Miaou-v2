using System;
using TMPro;
using UnityEngine;

public class GameScore : MonoBehaviour
{
    public static GameScore instance;
    public int[] scores;
    public string[] playerNames;
    public TMP_Text nameTable;
    public TMP_Text scoreTable;
    public int playerAmount;
    public string tempPlayerName;
    public int tempScore;
    public GameObject scoreMenu;

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
        if (scores.Length == 0)
        {
            scores = new int[playerAmount];
        }
    }

    public void AddScore(int scoreAdded)
    {
        tempScore += scoreAdded;
    }
    
    public void SetPlayerScore(int scoreSet)
    {
        tempScore = scoreSet;
    }

    public void ResetScore()
    {
        tempScore = 0;
    }

    public void UpdateBoard()
    {
        //checks if player score is greater than 10s place
        if (tempScore > scores[playerAmount])
        {
            
        }
        //sets player place to the minimum, checks for each score if it's greater to climb places
        int playerPlace = playerAmount;
        for (int i = playerAmount - 1; i >= 0; i--)
        {
            if (tempScore > scores[i])
            {
                playerPlace = i;
            }
        }
        //descend d'un cran tous les scores
        for (int i = playerAmount; i <= playerPlace; i--)
        {
            //copies score to lower place
            scores[i - 1] = scores[i];
            //destroys score
            scores[i] = 0;
        }
        //place le score dans son emplacement
        scores[playerPlace] = tempScore;
    }

    public void UpdateUI()
    {
        string tempPlayerTxt = String.Empty;
        for (int i = 0; i < playerAmount; i++)
        {
            tempPlayerTxt += playerNames[i] + "\n";
        }
        string tempScoresTxt = String.Empty;
        for (int i = 0; i < playerAmount; i++)
        {
            tempScoresTxt += scores[i] + "\n";
        }
    }

    public void OpenScoreMenu()
    {
        scoreMenu.SetActive(true);
    }

    public void CloseScoreMenu()
    {
        scoreMenu.SetActive(false);
    }
    //when death, or win : stores score in new float[i], which is stored in playerprefs
    //resets current score when starting game
    //table shows while UI screen
}
