using UnityEngine;
public class Achievements : MonoBehaviour
{
    public int noHitMaster;    // finir le jeu sans prendre de dégats
    public int noObject;     //     finir le jeu sans prendre d'objets
    public int pureSpeed;     //     finir le jeu en moins de 15min
    public int godlike;     // finir le jeu
    public int eyelionnaire;     //     avoir tant de thunes
    public GameObject[] achievementCards;
    public GameObject achievementPanel;

    public void ShowAchievementPanel()
    {
        //checks playerprefs for achievements
        noHitMaster = PlayerPrefs.GetInt("noHitMaster", 0);  
        noObject = PlayerPrefs.GetInt("noObject", 0); 
        pureSpeed = PlayerPrefs.GetInt("pureSpeed", 0); 
        godlike = PlayerPrefs.GetInt("godlike", 0); 
        eyelionnaire = PlayerPrefs.GetInt("eyelionnaire", 0);
        //then activates every unlocked achievement
        
        achievementPanel.SetActive(true);
        achievementCards[0].SetActive(noHitMaster == 1);
        achievementCards[1].SetActive(noObject == 1);
        achievementCards[2].SetActive(pureSpeed == 1);
        achievementCards[3].SetActive(godlike == 1);
        achievementCards[4].SetActive(eyelionnaire == 1);
    }

    public void DiscardAchievementPanel()
    {
        achievementPanel.SetActive(false);
    }
}
