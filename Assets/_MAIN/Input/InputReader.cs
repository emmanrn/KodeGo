using System;
using DIALOGUE;
using HISTORY;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Input Reader")]
public class InputReader : ScriptableObject, DialogueInput.IGeneralActions, DialogueInput.IPlayerActions, DialogueInput.IUIActions
{
    private DialogueInput dialogueInput;

    void OnEnable()
    {
        if (dialogueInput == null)
        {
            dialogueInput = new DialogueInput();

            dialogueInput.General.SetCallbacks(this);
            dialogueInput.Player.SetCallbacks(this);
            dialogueInput.UI.SetCallbacks(this);
        }

        SetPlayerMovement();
    }

    public void SetPlayerMovement()
    {
        dialogueInput.Player.Enable();
        dialogueInput.General.Disable();
        dialogueInput.UI.Disable();
    }
    public void SetGeneral()
    {
        dialogueInput.Player.Disable();
        dialogueInput.General.Enable();
        dialogueInput.UI.Disable();
    }
    public void SetUI()
    {
        dialogueInput.Player.Disable();
        dialogueInput.General.Disable();
        dialogueInput.UI.Enable();
    }

    public void Disable() => dialogueInput.Disable();
    public void Enable() => dialogueInput.Enable();


    public event Action<Vector2> MoveEvent;
    public event Action JumpPressed;
    public event Action JumpEvent;
    public event Action JumpCancelledEvent;
    public event Action DashEvent;
    public event Action PauseEvent;
    public event Action ResumeEvent;
    public event Action NextLevelEvent;
    public event Action PrevLevelEvent;
    public event Action InteractEvent;
    public event Action ToggleHistoryLogsEvent;
    public void OnCancel(InputAction.CallbackContext context)
    {
    }

    public void OnClick(InputAction.CallbackContext context)
    {
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            JumpPressed?.Invoke();

        if (context.phase == InputActionPhase.Performed)
            JumpEvent?.Invoke();

        if (context.phase == InputActionPhase.Canceled)
            JumpCancelledEvent?.Invoke();

    }

    public void OnMenuOPEN(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && !GeneralManager.instance.isPaused)
        {
            PauseEvent?.Invoke();
            SetGeneral();
        }
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && DialogueSystem.instance != null && DialogueSystem.instance.isRunningConversation && !GeneralManager.instance.isPaused)
            DialogueSystem.instance.OnUserPromptNext();
    }

    public void OnNextLevel(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            NextLevelEvent?.Invoke();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
    }

    public void OnPrevLevel(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            PrevLevelEvent?.Invoke();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
    }

    public void OnResume(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && GeneralManager.instance.isPaused)
        {
            ResumeEvent?.Invoke();
            if (DialogueSystem.instance.isRunningConversation)
            {
                SetGeneral();
                return;
            }

            SetPlayerMovement();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && !GeneralManager.instance.isPaused)
        {
            Debug.Log("Pauesd");
            PauseEvent?.Invoke();
            SetGeneral();
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            DashEvent?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            InteractEvent?.Invoke();
    }

    public void OnHistoryLog(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            ToggleHistoryLogsEvent?.Invoke();
    }
}
