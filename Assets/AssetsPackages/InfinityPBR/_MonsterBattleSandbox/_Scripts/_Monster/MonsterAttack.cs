using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MonsterAttack : MonoBehaviour
{
    [SerializeField] MonsterStateController monsterController;
    private BloodEffects bloodEffects;
    private float timeOfLastAttack;
    private float timeSinceLastAttack;
    private bool attackOnCooldown;
    private bool isFirstAttack;
    public event Action OnBaseAttack;
    private CharacterStats monsterStats;
    private AttackDefinition monsterBaseAttack;

    private void Start()
    {
        isFirstAttack = true;
        bloodEffects = GetComponent<BloodEffects>();
        monsterStats = GetComponent<CharacterStats>();
        monsterBaseAttack = monsterStats.GetBaseAttack();
    }

    public void Attack(float attackForce, float attackRate)
    {
        if (isFirstAttack == true)
        {
            transform.LookAt(monsterController.ChaseTarget.transform);
            timeOfLastAttack = Time.time;
            OnBaseAttack?.Invoke();
            isFirstAttack = false;
        }

        timeSinceLastAttack = Time.time - timeOfLastAttack;
        attackOnCooldown = timeSinceLastAttack < monsterBaseAttack.attackRate;

        if (monsterController.MonsterNavAgent.isActiveAndEnabled == true)
        {
            monsterController.MonsterNavAgent.isStopped = attackOnCooldown;
        }

        if (!attackOnCooldown)
        {
            transform.LookAt(monsterController.ChaseTarget.transform);
            timeOfLastAttack = Time.time;
            OnBaseAttack?.Invoke();
        }
    }

    public void Hit()
    {
        if (monsterController.ChaseTarget == null) { return; }
        if (monsterBaseAttack is Weapon)
        {
            bloodEffects?.BloodSplatterWithMyAttack();
            ((Weapon)monsterBaseAttack).ExecuteAttack(this.gameObject, monsterController.ChaseTarget.gameObject);
        }
    }
}
