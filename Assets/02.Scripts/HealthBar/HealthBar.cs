using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fillBar;
    [SerializeField] private Image virtualFillBar;
    [Space(10)]
    public float maxHealth;
    public float currentHealth;
    public float healThreshold;

    private void Start()
    {
        SetProgressBar(1);
    }

    private void LateUpdate()
    {
        UpdateProgressBar();
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        healThreshold = 0.1f * maxHealth;
    }

    public void Heal(float value)
    {
        currentHealth += value;
    }

    public void InitializeHealth(float value)
    {
        maxHealth = value;
        currentHealth = maxHealth;
    }

    public void SetProgressBar(int value)
    {
        fillBar.fillAmount = value;
        virtualFillBar.fillAmount = value;
    }

    public bool IsHealthLow()
    {
        return currentHealth <= healThreshold;
    }

    public void UpdateProgressBar()
    {
        fillBar.fillAmount = currentHealth / maxHealth;

        if (fillBar.fillAmount != virtualFillBar.fillAmount)
        {
            if (fillBar.fillAmount < virtualFillBar.fillAmount)
            {
                virtualFillBar.fillAmount -= Time.deltaTime * 0.1f;
                if (virtualFillBar.fillAmount < fillBar.fillAmount)
                    virtualFillBar.fillAmount = fillBar.fillAmount;
            }
            if (fillBar.fillAmount > virtualFillBar.fillAmount)
            {
                virtualFillBar.fillAmount += Time.deltaTime * 0.1f;
                if (virtualFillBar.fillAmount > fillBar.fillAmount)
                    virtualFillBar.fillAmount = fillBar.fillAmount;
            }
        }
    }
}
