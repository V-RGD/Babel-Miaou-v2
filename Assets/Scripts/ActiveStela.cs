using UnityEngine;
using UnityEngine.InputSystem;

public class ActiveStela : MonoBehaviour
{
    private PlayerControls playerControls;
    private MenuManager _menuManager;
    [HideInInspector]public InputAction collect;
    private Room room;
    private UIManager _uiManager;

    private GameObject player;
    private GameObject canvas;
    public GameObject activePrompt;

    private bool isPlayerInRange;
    private bool isActive;
    private float activeDist = 5;
    
    void ShopItem()
    {
        if (isPlayerInRange)
        {
            activePrompt.SetActive(true);
        }
        else
        {
            activePrompt.SetActive(false);
        }
            
        if ((player.transform.position - transform.position).magnitude <= activeDist)
        {
            //show message prompt
            isPlayerInRange = true;
        }
        else
        {
            //disable message prompt
            isPlayerInRange = false;
        }
    }
    void Collect(InputAction.CallbackContext context)
    {
        if (isPlayerInRange && !_menuManager.gameIsPaused)
        {
            //actives stela
        }
    }

    #region InputSystemRequirements
    private void OnEnable()
    {
        collect = playerControls.Player.Collect;
        collect.Enable();
        collect.performed += Collect;
    }

    private void OnDisable()
    {
        collect.Disable();
    }
    #endregion
}
