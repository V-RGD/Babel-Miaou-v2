#if  UNITY_EDITOR
using UnityEngine;
[ExecuteInEditMode]
public class ShowPlayerPrefs : MonoBehaviour
{
    public bool display;
    public int[] scoreList = new int[5];
    public string[] nameList = new string[5];
    void Update()
    {
        if (!display)
        {
            return;
        }

        display = false;

        scoreList[0] = PlayerPrefs.GetInt("Score1", 0);
        scoreList[1] = PlayerPrefs.GetInt("Score2", 0);
        scoreList[2] = PlayerPrefs.GetInt("Score3", 0);
        scoreList[3] = PlayerPrefs.GetInt("Score4", 0);
        scoreList[4] = PlayerPrefs.GetInt("Score5", 0);
        
        nameList[0] = PlayerPrefs.GetString("Name1", "undef");
        nameList[1] = PlayerPrefs.GetString("Name2", "undef");
        nameList[2] = PlayerPrefs.GetString("Name3", "undef");
        nameList[3] = PlayerPrefs.GetString("Name4", "undef");
        nameList[4] = PlayerPrefs.GetString("Name5", "undef");
    }
}
#endif
