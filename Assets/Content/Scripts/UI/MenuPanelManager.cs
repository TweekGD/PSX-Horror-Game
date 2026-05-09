using System;
using System.Collections.Generic;
using UnityEngine;

public enum PanelIndex
{
    MenuButtons = 0,
    Settings = 1
}

[System.Serializable]
public class UIPanel
{
    public SmoothAnimationUI animation;
    public Vector2 openPos;
    public Vector2 closePos;
    public float duration = 0.35f;
    public float delay = 0f;
    [HideInInspector] public bool isActive;
}

public class MenuPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject hudObject;
    [SerializeField] private GameObject menuContainer;
    [Space]
    [SerializeField] private UIPanel menuButtonsPanel;
    [SerializeField] private UIPanel settingsPanel;

    private Dictionary<PanelIndex, UIPanel> panels;

    private IInputManager inputManager;
    private IInputState inputState;
    public bool MenuIsOpen { get; private set; }

    private void Awake()
    {
        inputManager = ServiceLocator.Get<IInputManager>();
        inputState = ServiceLocator.Get<IInputState>();

        panels = new Dictionary<PanelIndex, UIPanel>
        {
            { PanelIndex.MenuButtons, menuButtonsPanel },
            { PanelIndex.Settings,    settingsPanel    }
        };
    }

    private void Update()
    {
        MenuInput();
    }

    private void MenuInput()
    {
        if (inputManager == null) return;

        if (inputManager.GetInput<bool>("PauseInput"))
        {
            MenuIsOpen = !MenuIsOpen;

            if (MenuIsOpen) OpenMenu();
            else CloseMenu();

            Debug.Log($"Menu panel state: {MenuIsOpen}");
        }
    }

    public void OpenMenu()
    {
        if (inputState.GetLockState(InputState.LockType.Menu)) { return; }

        MenuIsOpen = true;

        hudObject?.SetActive(false);
        menuContainer?.SetActive(true);

        if (inputState != null)
        {
            inputState.AddLock(InputState.LockType.Move, "PausePanel");
            inputState.AddLock(InputState.LockType.Camera, "PausePanel");
            inputState.RemoveLock(InputState.LockType.Cursor, "InitPlayer");
        }

        SetPanelActive(PanelIndex.MenuButtons, true);
    }

    public void CloseMenu()
    {
        CloseMenuPanels();

        SetPanelActive(PanelIndex.MenuButtons, false, onComplete: () =>
        {
            MenuIsOpen = false;

            menuContainer?.SetActive(false);
            hudObject?.SetActive(true);

            if (inputState != null)
            {
                inputState.RemoveLock(InputState.LockType.Move, "PausePanel");
                inputState.RemoveLock(InputState.LockType.Camera, "PausePanel");
                inputState.AddLock(InputState.LockType.Cursor, "InitPlayer");
            }
        });
    }

    public void ToggleSettingsPanel()
    {
        bool open = !IsActive(PanelIndex.Settings);
        CloseMenuPanels();
        SetPanelActive(PanelIndex.Settings, open);
    }

    private void SetPanelActive(PanelIndex index, bool isOpen, Action onComplete = null)
    {
        if (!HasPanel(index))
        {
            onComplete?.Invoke();
            return;
        }

        UIPanel panel = panels[index];
        panel.isActive = isOpen;

        Vector2 target = isOpen ? panel.openPos : panel.closePos;
        float delay = isOpen ? panel.delay : 0f;


        if (isOpen)
        {
            panel.animation.gameObject.SetActive(true);
            panel.animation.StartAnimationElement(target, panel.duration, delay, onComplete: onComplete);
        }
        else
        {
            panel.animation.StartAnimationElement(target, panel.duration, delay,
                deactivateOnComplete: true, onComplete: onComplete);
        }

    }

    private bool HasPanel(PanelIndex index) =>
        panels.TryGetValue(index, out var panel) && panel != null && panel.animation != null;

    private bool IsActive(PanelIndex index) =>
        HasPanel(index) && panels[index].isActive;

    private void CloseMenuPanels()
    {
        SetPanelActive(PanelIndex.Settings, false);
    }

    public void GameQuit() { Application.Quit(); }
}