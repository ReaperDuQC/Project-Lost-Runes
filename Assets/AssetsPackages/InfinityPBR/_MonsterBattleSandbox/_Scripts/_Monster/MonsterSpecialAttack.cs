using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MonsterSpecialAttack : MonoBehaviour
{
    [SerializeField] MonsterStateController monsterController;
    private BloodEffects bloodEffects;
    private float timeOfLastAttack;
    private float timeSinceLastAttack;
    private bool attackOnCooldown;
    private bool isFirstAttack;
    public event Action OnSpecialAttack;
    public event Action OnSpecialAttackHit;
    private CharacterStats monsterStats;
    private AttackDefinition monsterSpecialAttack;

    private void Start()
    {
        isFirstAttack = true;
        bloodEffects = GetComponent<BloodEffects>();
        monsterStats = GetComponent<CharacterStats>();
        monsterSpecialAttack = monsterStats.GetSpecialAttack();
    }

    public void Attack(float attackForce, float specialAttackCooldown)
    {
        if (monsterController.ChaseTarget == null) { return; }
        if (isFirstAttack == true)
        {
            transform.LookAt(monsterController.ChaseTarget.transform);
            timeOfLastAttack = Time.time;
            OnSpecialAttack?.Invoke();
            isFirstAttack = false;
            monsterController.MonsterNavAgent.isStopped = true;
            return;
        }

        timeSinceLastAttack = Time.time - timeOfLastAttack;
        attackOnCooldown = timeSinceLastAttack < specialAttackCooldown;

        if (monsterController.MonsterNavAgent.isActiveAndEnabled == true)
        {
            monsterController.MonsterNavAgent.isStopped = attackOnCooldown;
        }

        if (!attackOnCooldown)
        {
            if (monsterStats.IsDead == true) { return; }
            transform.LookAt(monsterController.ChaseTarget.transform);
            timeOfLastAttack = Time.time;
            OnSpecialAttack?.Invoke();
        }
    }

    public void Hit()
    {
        OnSpecialAttackHit?.Invoke();

        if (monsterSpecialAttack is AoeAttack)
        {
            //bloodEffects?.BloodSplatterWithMyAttack();
            ((AoeAttack)monsterSpecialAttack).FireAoeAttack(this.gameObject, this.gameObject.transform.position, LayerMask.NameToLayer("MonsterSpells"));
        }
    }
}
