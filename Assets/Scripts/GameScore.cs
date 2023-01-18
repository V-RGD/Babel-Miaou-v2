using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameScore : MonoBehaviour
{
    public static GameScore instance;
    public string tempName;
    public int tempScore;
    public TMP_Text currentScore;
    public TMP_Text[] users;
    public TMP_Text[] scoreTxt;
    public TMP_InputField nameInput;
    public GameObject leaderboardMenu;
    public Button mainMenuButton;
    public bool playerEnteredName;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
    }

    public IEnumerator ShowLeaderBoards()
    {
        playerEnteredName = false;
        //shows leaderboard screen
        leaderboardMenu.SetActive(true);
        //freezes player and game
        Time.timeScale = 0;
        PlayerController.instance.enabled = false;
        PlayerAttacks.instance.enabled = false;
        mainMenuButton.enabled = false;
        nameInput.gameObject.SetActive(true);
        currentScore.text = tempScore.ToString();
        //updates scores
        int[] scoreList = new int[5];
        int[] oldScoreList = scoreList;
        scoreList[0] = PlayerPrefs.GetInt("Score1");
        scoreList[1] = PlayerPrefs.GetInt("Score2");
        scoreList[2] = PlayerPrefs.GetInt("Score3");
        scoreList[3] = PlayerPrefs.GetInt("Score4");
        scoreList[4] = PlayerPrefs.GetInt("Score5");
        for (int i = 0; i < 5; i++)
        {
            scoreTxt[i].text = scoreList[i].ToString();
        }
        //updates names
        string[] nameList = new string[5];
        string[] oldNames = nameList;
        nameList[0] = PlayerPrefs.GetString("Name1");
        nameList[1] = PlayerPrefs.GetString("Name2");
        nameList[2] = PlayerPrefs.GetString("Name3");
        nameList[3] = PlayerPrefs.GetString("Name4");
        nameList[4] = PlayerPrefs.GetString("Name5");
        for (int i = 0; i < 5; i++)
        {
            users[i].text = nameList[i];
        }
        
        //checks if user score is in the top 5
        int scorePosition = 5; //score is by default lower than each score in the leaderboard
        for (var i = 4; i > -1; i--) //checks if the score is higher than each rank
        {
            if (tempScore > scoreList[i])
            {
                scorePosition--;
            }
        }
        Debug.Log(scorePosition);

        //if score is in the leaderboard
        if (scorePosition < 5)
        {
            //wait till player adds it's name
            yield return new WaitUntil(() => playerEnteredName);

            //once it's done, applies score and decreases each other score below
            int tempDelayedScore = scoreList[scorePosition];
            string tempDelayedName = nameList[scorePosition];
            //applies current score to the position
            scoreList[scorePosition] = tempScore;
            nameList[scorePosition] = nameInput.text;

            //if temp delayed score can be in the leaderboard, add it
            if (scorePosition + 1 < 5)
            {
                scoreList[scorePosition + 1] = tempDelayedScore;
                nameList[scorePosition + 1] = tempDelayedName;
            }

            //then decreases every other score
            for (int i = 5 + 1; i < scorePosition + 1; i--)
            {
                scoreList[i + 1] = oldScoreList[i];
                nameList[i + 1] = oldNames[i];
            }

            PlayerPrefs.SetInt("Score1", scoreList[0]);
            PlayerPrefs.SetInt("Score2", scoreList[1]);
            PlayerPrefs.SetInt("Score3", scoreList[2]);
            PlayerPrefs.SetInt("Score4", scoreList[3]);
            PlayerPrefs.SetInt("Score5", scoreList[4]);
            
            PlayerPrefs.SetString("Name1", nameList[0]);
            PlayerPrefs.SetString("Name2", nameList[1]);
            PlayerPrefs.SetString("Name3", nameList[2]);
            PlayerPrefs.SetString("Name4", nameList[3]);
            PlayerPrefs.SetString("Name5", nameList[4]);
            
            //updates scores
            scoreList[0] = PlayerPrefs.GetInt("Score1");
            scoreList[1] = PlayerPrefs.GetInt("Score2");
            scoreList[2] = PlayerPrefs.GetInt("Score3");
            scoreList[3] = PlayerPrefs.GetInt("Score4");
            scoreList[4] = PlayerPrefs.GetInt("Score5");
            for (int i = 0; i < 5; i++)
            {
                scoreTxt[i].text = scoreList[i].ToString();
            }
            //updates names
            nameList[0] = PlayerPrefs.GetString("Name1");
            nameList[1] = PlayerPrefs.GetString("Name2");
            nameList[2] = PlayerPrefs.GetString("Name3");
            nameList[3] = PlayerPrefs.GetString("Name4");
            nameList[4] = PlayerPrefs.GetString("Name5");
            for (int i = 0; i < 5; i++)
            {
                users[i].text = nameList[i];
            }

            //return to menu
            mainMenuButton.enabled = true;
            nameInput.gameObject.SetActive(false);
        }
        else
        {
            //enables button to go back to the main menu
            mainMenuButton.enabled = true;
            nameInput.gameObject.SetActive(false);
        }
    }
    
    public void TypeName()
    {
        //get name from field
        playerEnteredName = true;
    }

    public void AddScore(int scoreAdded)
    {
        tempScore += scoreAdded;
        currentScore.text = tempScore.ToString();
    }
}
