using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackedThenInstantDeath : MonoBehaviour, IAttackable
{
    private CharacterStats characterStats;
    private MonsterSpecialAttack specialAttack;

    private void Start()
    {
        characterStats = GetComponent<CharacterStats>();
        specialAttack = GetComponent<MonsterSpecialAttack>();
    }

    public void OnAttack(GameObject attacker, Attack attack)
    {
        characterStats.TakeDamage(characterStats.GetHealth());
    }
}
