using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameScore : MonoBehaviour
{
    public static GameScore instance;
    public int tempScore;
    public TMP_Text currentScore;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
    }

    public GameObject leaderboardMenu;
    public TMP_Text[] scoreTxt;
    public bool playerEnteredName;
    public Button mainMenuButton;
    public GameObject enterName;

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
        enterName.SetActive(true);
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

        //checks if user score is in the top 5
        int scorePosition = 5; //score is by default lower than each score in the leaderboard
        for (var i = 4; i > 0; i--) //checks if the score is higher than each rank
        {
            if (GameScore.instance.tempScore > scoreList[i])
            {
                scorePosition--;
            }
        }

        //if score is in the leaderboard
        if (scorePosition < 5)
        {
            //wait till player adds it's name
            yield return new WaitUntil(() => playerEnteredName);

            //once it's done, applies score and decreases each other score below
            int tempDelayedScore = scoreList[scorePosition];
            //applies current score to the position
            scoreList[scorePosition] = GameScore.instance.tempScore;

            //if temp delayed score can be in the leaderboard, add it
            if (scorePosition + 1 < 5)
            {
                scoreList[scorePosition + 1] = tempDelayedScore;
            }

            //then decreases every other score
            for (int i = 5 + 1; i < scorePosition + 1; i--)
            {
                scoreList[i + 1] = oldScoreList[i];
            }

            //updates everything
            for (int i = 0; i < 5; i++)
            {
                scoreTxt[i].text = scoreList[i].ToString();
            }

            PlayerPrefs.SetInt("Score1", scoreList[0]);
            PlayerPrefs.SetInt("Score2", scoreList[1]);
            PlayerPrefs.SetInt("Score3", scoreList[2]);
            PlayerPrefs.SetInt("Score4", scoreList[3]);
            PlayerPrefs.SetInt("Score5", scoreList[4]);

            //return to menu
            mainMenuButton.enabled = true;
            enterName.SetActive(false);
        }
        else
        {
            //enables button to go back to the main menu
            mainMenuButton.enabled = true;
            enterName.SetActive(false);
        }
    }

    public TMP_InputField nameField;
    public TMP_Text[] nameTxt;

    public void TypeName()
    {
        //get name from field

        //sets name text
        //enables the process
        playerEnteredName = true;
    }

    public void AddScore(int scoreAdded)
    {
        tempScore += scoreAdded;
        currentScore.text = tempScore.ToString();
    }
}
