using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberBugEffects : MonoBehaviour
{
    [SerializeField] MonsterStateController bomberBugController;
    [SerializeField] private Renderer bomberBugButtRenderer;
    private CharacterStats bomberBugStats;
    private MonsterSpecialAttack monsterSpecialAttack;

    private void Start()
    {
        bomberBugStats = GetComponent<CharacterStats>();
        bomberBugController = GetComponent<MonsterStateController>();
        monsterSpecialAttack = GetComponent<MonsterSpecialAttack>();

        bomberBugController.OnMonsterStateChange += BomberBugChaseEffect;
        monsterSpecialAttack.OnSpecialAttack += AttackButtFlash;
        monsterSpecialAttack.OnSpecialAttackHit += SelfDestruct;
    }

    private void BomberBugChaseEffect(MonsterState currentState)
    {
        bomberBugButtRenderer.material.SetFloat("_FlashSpeed", 10f);
        bomberBugButtRenderer.material.SetColor("_FlashColor", new Color(1.0f, 0.64f, 0.0f));
    }

    private void SelfDestruct()
    {
        bomberBugButtRenderer.material.SetFloat("_FlashSpeed", .5f);
        bomberBugButtRenderer.material.SetColor("_FlashColor", Color.black);
        StartCoroutine(TakeAllHealth());
    }

    IEnumerator TakeAllHealth()
    {
        yield return new WaitForSeconds(1);
        bomberBugButtRenderer.material.SetFloat("_FlashSpeed", .5f);
        bomberBugButtRenderer.material.SetColor("_FlashColor", Color.black);
        int currentHealth;
        currentHealth = bomberBugStats.GetHealth();
        bomberBugStats.TakeDamage(currentHealth);
    }

    private void AttackButtFlash()
    {
        bomberBugController.MonsterNavAgent.isStopped = true;
        bomberBugButtRenderer.material.SetFloat("_FlashSpeed", 20f);
        bomberBugButtRenderer.material.SetColor("_FlashColor", Color.red);
    }

    private void OnDisable()
    {
        monsterSpecialAttack.OnSpecialAttack -= AttackButtFlash;
    }
}
