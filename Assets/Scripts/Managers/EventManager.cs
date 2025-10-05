using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static EventManager current;

    private void Awake()
    {
        current = this;
    }

    #region Game State Control
        public event Action GameStarted;
        public void OnGameStarted() => GameStarted?.Invoke();

        public event Action GameEnded;
        public void OnGameEnded() => GameEnded?.Invoke();

        public event Action GameExit;
        public void OnGameExit() => GameExit?.Invoke();

        public event Action GamePause;
        public void OnGamePause() => GamePause?.Invoke();

        public event Action GameUnPause;
        public void OnGameUnPause() => GameUnPause?.Invoke();

        public event Action GameIncrementScore;
        public void OnGameIncrementScore() => GameIncrementScore?.Invoke();

        public event Action GameIncrementFloristryScore;
        public void OnGameIncrementFloristryScore() => GameIncrementFloristryScore?.Invoke();
    #endregion

    #region Player Controls

    public event Action EnableInput;
    public void OnEnableInput() => EnableInput?.Invoke();

    #endregion

    #region Dialogue Controls
    public event Action<NPCController, PlayerController> InitializeDialogue;
    public void OnInitializeDialogue(NPCController npc, PlayerController player) => InitializeDialogue?.Invoke(npc, player);

    #endregion

    #region UI Controls
    public event Action<NPCController, PlayerController> ActivateDialogue;
    public void OnActivateDialogue(NPCController npc, PlayerController player) => ActivateDialogue?.Invoke(npc, player);

    public event Action DeactivateDialogue;
    public void OnDeactivateDialogue() => DeactivateDialogue?.Invoke();

    public event Action ToggleChatLog;
    public void OnToggleChatLog() => ToggleChatLog?.Invoke();

    public event Action ToggleCart;
    public void OnToggleCart() => ToggleCart?.Invoke();
    #endregion
}

