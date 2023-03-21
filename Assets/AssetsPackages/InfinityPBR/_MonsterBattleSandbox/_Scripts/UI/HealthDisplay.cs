using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private CharacterStats stats;
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBarImage = null;
    private int maxHealth;
    private bool isSelected;

    private void Awake()
    {
        stats.OnHealthUpdated += HandleHealthUpdated;
    }

    private void Start()
    {
        stats = GetComponent<CharacterStats>();
        maxHealth = stats.GetMaxHealth();
    }

    private void OnDestroy()
    {
        stats.OnHealthUpdated -= HandleHealthUpdated;
    }

    private void OnMouseEnter()
    {
        healthBarParent.SetActive(true);
    }

    private void OnMouseExit()
    {
        StartCoroutine(ShowHealthBar());
    }

    private void HandleHealthUpdated(int currentHealth)
    {
        StartCoroutine(ShowHealthBar());
        maxHealth = stats.GetMaxHealth();
        if (currentHealth <= 0) { currentHealth = 0; }
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }

    public void ShowHealthBarOnSelected()
    {
        isSelected = true;
        healthBarParent.SetActive(true);
    }

    public void DisableHealthBarOnDeselected()
    {
        isSelected = false;
        healthBarParent.SetActive(false);
    }

    IEnumerator ShowHealthBar()
    {
        if (isSelected) { yield break; };
        healthBarParent.SetActive(true);
        yield return new WaitForSeconds(4);
        if (isSelected) { yield break; };
        healthBarParent.SetActive(false);
    }
}
