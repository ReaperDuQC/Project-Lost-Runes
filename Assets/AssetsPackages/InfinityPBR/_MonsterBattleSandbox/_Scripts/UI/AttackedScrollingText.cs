using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackedScrollingText : MonoBehaviour, IAttackable
{
    public GameObject scrollingTextPrefab;
    [SerializeField] bool useCharacterColor = true;
    public Color textColor;
    private Camera mainCamera;

    private void OnEnable()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (useCharacterColor)
        {
            CharacterStats stats = GetComponent<CharacterStats>();
            textColor = stats.GetCharacterColor();
        }
    }

    public void OnAttack(GameObject attacker, Attack attack)
    {
        var text = attack.Damage.ToString();
        var scrollingTextInstantiated = Instantiate(scrollingTextPrefab, transform.position, mainCamera.transform.rotation);
        ScrollingText scrollingText = scrollingTextInstantiated.GetComponent<ScrollingText>();
        scrollingText.SetText("-" + text);
        scrollingText.SetColor(textColor);
    }
}
