using UnityEngine;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu instance;

        [SerializeField] private GameObject panel;

        public void Pause()
        {
            Time.timeScale = 0;
            panel.SetActive(true);
        }

        public void Resume()
        {
            Time.timeScale = 1;
            panel.SetActive(false);
        }
        
        public void ClickButton()
        {
            
        }
    }
}
