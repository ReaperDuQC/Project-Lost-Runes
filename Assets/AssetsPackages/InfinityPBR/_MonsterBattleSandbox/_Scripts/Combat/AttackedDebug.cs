using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackedDebug : MonoBehaviour, IAttackable
{
    private CharacterStats stats;
    string debugCritical;
    string closeDebugCritical;

    private void Awake()
    {
        stats = GetComponent<CharacterStats>();
        debugCritical = "<color=orange>"; 
        closeDebugCritical = "</color>";
    }

    public void OnAttack(GameObject attacker, Attack attack)
    {
        if (attack.IsCritical)
        {
            Debug.Log
                (debugCritical 
                + attacker.name + " CRITICAL!"
                + closeDebugCritical);
        }
        Debug.LogFormat
            (
            "{0} <color=red> ATTACKED </color>" 
            + "{1} <color=red> for {2} DAMAGE.</color>"
            , attacker.name, name, attack.Damage
            );
        Debug.LogFormat
            ( "{1} " + stats.GetHealth()
            + "<color=red> \u2665 </color>"
            // + " REMAINING."
            , attacker.name, name, attack.Damage
            );
    }
}
