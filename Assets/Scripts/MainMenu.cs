using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    
    public bool isLoading;
    public bool quitWarningActive;
    public bool isInOptions;

    public GameObject loadingSlider;
    public GameObject quitWarning;
    public GameObject loadingUI;
    public GameObject optionMenu;
    
    private bool _canActiveDeathPanel = true;
    private bool _canPause = true;
    public bool canEscapeObjectMenu = true;
    private bool isInObjectMenu;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        instance = this;
    }

    #region fonctionne
    public void StartGame()
    {
        StartCoroutine(LoadingScreen());
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void SettingsMenu()
    {
        if (!isInOptions)
        {
            isInOptions = true;
            optionMenu.SetActive(true);
        }
        else
        {
            isInOptions = false;
            optionMenu.SetActive(false);
        }
    }
    #endregion
    
    #region WarningPrompt
    #endregion
    IEnumerator LoadingScreen()
    {
        loadingUI.SetActive(true);
        //does nothing the first second
        yield return new WaitForSeconds(1);
        loadingSlider.GetComponent<Animator>().SetTrigger("LoadingStart");
        //loads for 3 seconds
        yield return new WaitForSeconds(0.1f);
        //change scene
        SceneManager.LoadScene("MainScene");
    }
    public void DiscardWarningPrompt()
    {
        quitWarningActive = false;
        quitWarning.SetActive(false);
    }
}
