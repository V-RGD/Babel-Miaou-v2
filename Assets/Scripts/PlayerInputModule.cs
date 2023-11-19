using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputModule : MonoBehaviour
{
    [SerializeField] ButtonInput[] inputs; //list of inputs
    HashSet<ButtonInput.Interaction> _holdInteractions; // the list of inputs that have to be held

    [Serializable]
    public class ButtonInput : Input
    {
        [SerializeField] internal Interaction[] interactions; //a list of interactions triggered by the input

        [Serializable]
        public class Interaction
        {
            [SerializeField] internal InteractionType interactionType;

            public enum InteractionType
            {
                OnClick,
                AfterHeldFor,
                OnRelease
            }

            #region Hold

            internal void StartHoldingInput(InputAction.CallbackContext ctx) => InputHeld = true;
            internal void StopHoldingInput(InputAction.CallbackContext ctx) => InputHeld = false;

            [SerializeField]
            internal float holdTime;

            internal bool InputHeld;
            internal float HoldTimer;

            #endregion

            [SerializeField] internal UnityEvent Action;
            internal void TriggerEvent(InputAction.CallbackContext ctx) => Action.Invoke();
        }
    }

    [Serializable]
    public class Input
    {
        [SerializeField] internal InputActionReference inputAction;
    }

    public void Start()
    {
        _holdInteractions = new HashSet<ButtonInput.Interaction>();

        //enables each input
        foreach (var input in inputs)
        {
            input.inputAction.action.Enable();

            foreach (var interaction in input.interactions)
            {
                //assigns each delegate to its corresponding input
                switch (interaction.interactionType)
                {
                    case ButtonInput.Interaction.InteractionType.OnClick:
                        //triggers the event whenever the input is pressed
                        input.inputAction.action.performed += interaction.TriggerEvent;
                        break;

                    case ButtonInput.Interaction.InteractionType.AfterHeldFor:
                        //manages when the input is pressed and released
                        input.inputAction.action.performed += interaction.StartHoldingInput;
                        input.inputAction.action.canceled += interaction.StopHoldingInput;
                        _holdInteractions.Add(interaction);
                        break;

                    case ButtonInput.Interaction.InteractionType.OnRelease:
                        //triggers the event whenever the input is released
                        input.inputAction.action.canceled += interaction.TriggerEvent;
                        break;
                }
            }
        }
    }

    public void Update()
    {
        //syncs the input system update to the network update
        InputSystem.Update();

        //for the "hold" type interactions, checks if the input is pressed or released and updates the timers
        //then triggers their respective function if the timer exceeds the hold time specified
        foreach (var interaction in _holdInteractions)
        {
            //checks if the input is pressed, and if so increases the timer
            if (interaction.InputHeld)
            {
                interaction.HoldTimer += Time.deltaTime;

                //when the timer exceeds the hold time specified, triggers the function
                if (!(interaction.HoldTimer >= interaction.holdTime)) continue;

                interaction.HoldTimer = 0;
                interaction.InputHeld = false;
                interaction.Action.Invoke();
            }
            else
            {
                interaction.HoldTimer = 0;
            }
        }
    }

    public void OnDisable()
    {
        //disables each input
        foreach (var input in inputs)
        {
            input.inputAction.action.Disable();

            foreach (var interaction in input.interactions)
            {
                //unsubscribes each function from its delegate
                switch (interaction.interactionType)
                {
                    case ButtonInput.Interaction.InteractionType.OnClick:
                        input.inputAction.action.performed -= interaction.TriggerEvent;
                        break;

                    case ButtonInput.Interaction.InteractionType.AfterHeldFor:
                        input.inputAction.action.performed -= interaction.StartHoldingInput;
                        input.inputAction.action.canceled -= interaction.StopHoldingInput;
                        break;

                    case ButtonInput.Interaction.InteractionType.OnRelease:
                        input.inputAction.action.canceled -= interaction.TriggerEvent;
                        break;
                }
            }
        }
    }
}