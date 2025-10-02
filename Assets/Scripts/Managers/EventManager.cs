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
    #endregion

    #region Player Controls

    // examples of how to construct events that can be called from other scripts.

    /*public event Action GameStarted;
    public void OnGameStarted() => GameStarted?.Invoke();

    public event Action<GameObject, bool> TrackPeopleKilled;
    public void OnTrackPeopleKilled(GameObject casualty, bool playerShot) => TrackPeopleKilled?.Invoke(casualty, playerShot);*/


    #endregion
}

