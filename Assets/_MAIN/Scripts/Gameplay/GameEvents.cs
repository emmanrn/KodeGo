using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action OnPlayerDied;
    public static event Action OnBlockCollected;

    public static void PlayerDied()
    {
        OnPlayerDied?.Invoke();
    }

}
