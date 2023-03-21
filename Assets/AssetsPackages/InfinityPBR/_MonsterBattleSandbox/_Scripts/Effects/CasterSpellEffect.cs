using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasterSpellEffect : MonoBehaviour
{
    public GameObject spellEffect;
    private Unit unit;

    private void Start()
    {
        unit = GetComponent<Unit>();
        spellEffect.SetActive(false);
        unit.OnSpellCast += DisableEffect;
        unit.OnNewCommand += DisableEffect;
    }

    private void OnDisable()
    {
        unit.OnSpellCast -= DisableEffect;
        unit.OnNewCommand -= DisableEffect;
    }

    public void EnableEffect()
    {
        spellEffect.SetActive(true);
    }

    public void DisableEffect()
    {
        spellEffect.SetActive(false);
    }
}
