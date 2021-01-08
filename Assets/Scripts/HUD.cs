using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Slider healthBar;

    private void Start()
    {
        EventSystem.onHealthUpdate += HealthUpdate;
    }

    void HealthUpdate(int health)
    {
        healthBar.value = health;
    }
}
