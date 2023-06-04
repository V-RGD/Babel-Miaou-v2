using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LeaderBoardsLogic : MonoBehaviour
{
    public bool saved;
    public float currentScore;
    public List<float> scoreBoard;
    public List<float> editedScoreBoard;
    void Start()
    {
        scoreBoard.Clear();
        for (int i = 0; PlayerPrefs.HasKey("score" + i); i++)
        {
            scoreBoard.Add(PlayerPrefs.GetFloat("score" + i));
        }
    }

    void CompareScores()
    {
        editedScoreBoard.Clear();
        saved = false;
        if (scoreBoard.Count == 0)
        {
            editedScoreBoard.Add(currentScore);
        }
        else
        {
            for (int i = 0; i < scoreBoard.Count(); i++)
            {
                if (currentScore < scoreBoard[i])
                {
                    //Debug.Log("currentScore lower to score" + i);
                }
                editedScoreBoard.Add(scoreBoard[i]);
            }
        
            if (saved == false )
            {
                editedScoreBoard.Add(currentScore);
                saved = true;
            }
        }
        
    }

    void SaveScores()
    {
        PlayerPrefs.DeleteAll();
        for (int i = 0; i < editedScoreBoard.Count(); i++)
        {
            PlayerPrefs.SetFloat("score" + i, editedScoreBoard[i]);
            //Debug.Log("saved" + i);
        }
        Start();
        
    }

    void OverwriteScores()
    {
        CompareScores();
        SaveScores();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OverwriteScores();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            PlayerPrefs.DeleteAll();
            editedScoreBoard.Clear();
            scoreBoard.Clear();
        }
        else if(Input.GetMouseButtonDown(2))
        {
            for (int i = 0; PlayerPrefs.HasKey("score" + i); i++)
            {
                Debug.Log("score" + i);
            }
        }
    }
}
