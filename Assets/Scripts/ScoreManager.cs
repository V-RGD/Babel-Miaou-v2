using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public bool showScore = false;
    private float score;
    private float elapsedTime;
    private float perSecond;
    void Start()
    { 
        elapsedTime = 0;
    }
    
    void Update()
    {
        TimePass(perSecond);
        SeeScoreText(showScore);
    }

    void TimePass(float second)
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= second)
        {
            elapsedTime = 0;
            if (score !< 0)
            {
                score -= 1;
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
    
}
