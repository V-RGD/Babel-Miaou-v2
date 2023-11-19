using UnityEngine;
using TMPro;
public class ScoreManager_old : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private bool countingDown;
    public bool showScore = false;
    public float score;
    private float elapsedTime;
    private float perSecond;
    void Start()
    { 
        elapsedTime = 0;
        perSecond = 1;
    }
    
    void Update()
    {
        TimePass(perSecond);
        SeeScoreText(showScore);
    }

    void TimePass(float second)
    {
        if (countingDown)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= second)
            {
                elapsedTime = 0;
                if (score > 0)
                {
                    score -= 1;
                }
            }
        }
    }

    public void AddScore(float addedScore)
    {
        score += addedScore;
    }

    void SeeScoreText(bool aaa)
    {
        if (aaa && scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void StopCounting()
    {
        countingDown = false;
    }
    
}
