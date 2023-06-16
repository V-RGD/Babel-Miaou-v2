using System;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputManager : MonoBehaviour
    {
        PlayerControls _inputActions;

        InputAction _move;
        InputAction _dash;
        InputAction _confirm;
        InputAction _menu;
        InputAction _attack;
        
        public enum PlayMode
        {
            Game,
            Menu
        }
        
        public enum MenuMode
        {
            Pause,
            ItemDraw,
            ItemReplace,
            MainMenu
        }

        public PlayMode playMode;
        public MenuMode menuMode;

        void Awake()
        {
            _inputActions = new PlayerControls();
        }

        void Update()
        {
            //updates player direction depending on input
            Vector2 dir = _move.ReadValue<Vector2>();
            Controller.instance.direction = new Vector3(dir.x, 0, dir.y);
        }

        void InputDash(InputAction.CallbackContext context)
        {
            if (playMode == PlayMode.Game) Controller.instance.InputDash();
        }

        void InputAttack(InputAction.CallbackContext context)
        {
            if (playMode == PlayMode.Game) Attacks.instance.InputAttack();
        }

        void InputConfirm(InputAction.CallbackContext context)
        {
            if (playMode == PlayMode.Game) return;
            switch (menuMode)
            {
                case MenuMode.Pause:
                    UI.PauseMenu.instance.ClickButton();
                    break;
                case MenuMode.ItemDraw:
                    UI.ItemDraw.instance.ChooseItem();
                    break;
                case MenuMode.ItemReplace:
                    UI.ItemReplace.instance.Confirm();
                    break;
                case MenuMode.MainMenu:
                    UI.MainMenu.instance.ClickButton();
                    break;
            }
        }

        void InputMenu(InputAction.CallbackContext context)
        {
            //if game is running, pauses it
            if (playMode == PlayMode.Game)
            {
                UI.PauseMenu.instance.Pause();
                return;
            }
            //if the player is in a menu and if that menu is the pause menu, resumes game
            switch (menuMode)
            {
                case MenuMode.Pause:
                    UI.PauseMenu.instance.Resume();
                    break;
            }
        }

        void OnEnable()
        {
            _move = _inputActions.Player.Move;
            _dash = _inputActions.Player.Dash;
            _confirm = _inputActions.Menus.Confirm;
            _menu = _inputActions.Menus.Menu;
            _attack = _inputActions.Player.LightAttack;

            _dash.performed += InputDash;
            _confirm.performed += InputConfirm;
            _menu.performed += InputMenu;
            _attack.performed += InputAttack;
            
            _move.Enable();
            _dash.Enable();
            _confirm.Enable();
            _menu.Enable();
            _attack.Enable();
        }

        void OnDisable()
        {
            _inputActions.Disable();
        }
    }
}
