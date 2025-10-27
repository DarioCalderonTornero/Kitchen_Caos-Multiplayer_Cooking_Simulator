using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternativeAction;
    public event EventHandler OnGamePaused;

    private PlayerInputActions playerInputActions;

    public event EventHandler OnRebinBinding;

    public static PlayerInputs Instance { get; private set; }

    public enum Binding { moveUp, moveDown, moveLeft, moveRight, interact, interactAlt, pause, gamepadInteract, gamepadInteractAlt, gamepadPause }


    private void Awake()
    {
        playerInputActions = new PlayerInputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }


        playerInputActions.Player.Enable();

        Instance = this;

        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternative.performed += InteractAlternative_performed;
        playerInputActions.Player.GamePaused.performed += GamePaused_performed;

        //Debug.Log(GetBinding(Binding.interact));
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= Interact_performed;
        playerInputActions.Player.InteractAlternative.performed -= InteractAlternative_performed;
        playerInputActions.Player.GamePaused.performed -= GamePaused_performed;

        playerInputActions.Dispose();
    }

    private void GamePaused_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnGamePaused?.Invoke(this, EventArgs.Empty);
    }

   

    private void InteractAlternative_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlternativeAction?.Invoke(this, EventArgs.Empty);
    }
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementNormalized()
    {
        Vector2 inputMovement = playerInputActions.Player.PlayerMovement.ReadValue<Vector2>();
        inputMovement = inputMovement.normalized;
        return inputMovement;
    }

    public string GetBinding(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.interact:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.interactAlt:
                return playerInputActions.Player.InteractAlternative.bindings[0].ToDisplayString();
            case Binding.pause:
                return playerInputActions.Player.GamePaused.bindings[0].ToDisplayString();
            case Binding.moveUp:
                return playerInputActions.Player.PlayerMovement.bindings[1].ToDisplayString();
            case Binding.moveDown:
                return playerInputActions.Player.PlayerMovement.bindings[2].ToDisplayString();
            case Binding.moveLeft:
                return playerInputActions.Player.PlayerMovement.bindings[3].ToDisplayString();
            case Binding.moveRight:
                return playerInputActions.Player.PlayerMovement.bindings[4].ToDisplayString();
            case Binding.gamepadInteract:
                return playerInputActions.Player.Interact.bindings[1].ToDisplayString();
            case Binding.gamepadInteractAlt:
                return playerInputActions.Player.InteractAlternative.bindings[1].ToDisplayString();
            case Binding.gamepadPause:
                return playerInputActions.Player.GamePaused.bindings[1].ToDisplayString();


        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        playerInputActions.Player.Disable();

        InputAction inputAction;
        int rebingIndex;

        switch(binding)
        {
            default:

            case Binding.moveUp:
                inputAction = playerInputActions.Player.PlayerMovement;
                rebingIndex = 1;
                break;

            case Binding.moveDown:
                inputAction = playerInputActions.Player.PlayerMovement;
                rebingIndex = 2;
                break;

            case Binding.moveLeft:
                inputAction = playerInputActions.Player.PlayerMovement;
                rebingIndex = 3;
                break;

            case Binding.moveRight:
                inputAction = playerInputActions.Player.PlayerMovement;
                rebingIndex = 4;
                break;

            case Binding.interact:
                inputAction = playerInputActions.Player.Interact;
                rebingIndex = 0;
                break;

            case Binding.interactAlt:
                inputAction = playerInputActions.Player.InteractAlternative;
                rebingIndex = 0;
                break;

            case Binding.pause:
                inputAction = playerInputActions.Player.GamePaused;
                rebingIndex = 0;
                break;

            case Binding.gamepadInteract:
                inputAction = playerInputActions.Player.Interact;
                rebingIndex = 1;
                break;

            case Binding.gamepadInteractAlt:
                inputAction = playerInputActions.Player.InteractAlternative;
                rebingIndex = 1;
                break;

            case Binding.gamepadPause:
                inputAction = playerInputActions.Player.GamePaused;
                rebingIndex = 1;
                break;

        }


        inputAction.PerformInteractiveRebinding(rebingIndex).OnComplete(callback =>
        {
            callback.Dispose();
            playerInputActions.Player.Enable();
            onActionRebound();

            PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();

            OnRebinBinding?.Invoke(this, EventArgs.Empty);
        })
        .Start();

    }
}

