using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem
{
    public delegate void intAction(int i);
    public static event intAction onHealthUpdate;
    public static event Action onPlayerDeath;

    public static void HealthUpdate(int amount)
    {
        onHealthUpdate?.Invoke(amount);
    }

    public static void PlayerDeath()
    {
        onPlayerDeath?.Invoke();
    }
}
