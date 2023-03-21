using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

/* ---About Unit---
 - Handles unit movement
 - Handles unit attacks
 - Similar to the MonsterStateController, only this doesn't directly
   control the units because this responds to MouseManager
   and UnitCommand
*/

public class Unit : MonoBehaviour
{
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;
    public float ChaseRange = 5;

    private GameObject attackTarget;
    private float attackRate;
    [SerializeField] private Transform attackImpactTransform;
    [SerializeField] private LayerMask layerForMySpells;
    private int layerIndexForMySpells;

    private NavMeshAgent unitNavAgent;
    private CharacterStats unitStats;
    private CharacterStats monsterStats;

    private AttackDefinition unitBaseAttack;
    private float timeOfLastAttack;
    private float timeSinceLastAttack;
    private bool attackOnCooldown;

    public static event Action<Unit> OnUnitSpawned;
    public static event Action<Unit> OnUnitDespawned;
    public event Action OnNewCommand;
    public event Action OnUnitAttack;
    public event Action OnSpellCast;

    private void Awake()
    {
        unitNavAgent = GetComponent<NavMeshAgent>();
        unitStats = GetComponent<CharacterStats>();
        timeOfLastAttack = float.MinValue;
    }

    void Start()
    {
        unitNavAgent.speed = unitStats.GetNavAgentSpeed();
        OnUnitSpawned?.Invoke(this);
        unitStats.OnCharacterDeath += HandleDeath;
        ChaseRange = unitStats.GetAttackRange() + (unitStats.GetAttackRange() / 3f);
        layerIndexForMySpells = LayerMask.NameToLayer("UnitSpells");
    }

    void OnDisable()
    {
        OnUnitDespawned?.Invoke(this);
        unitStats.OnCharacterDeath -= HandleDeath;
    }

    private void Update()
    {
        if (unitStats.IsDead == true) { return; }
        if (attackTarget == null) { return; }
        if (IsTargetDead()) { return; }
        timeSinceLastAttack = Time.time - timeOfLastAttack;
        attackOnCooldown = timeSinceLastAttack < attackRate;

        if (!attackOnCooldown && IsInChaseRange())
        {
            timeOfLastAttack = Time.time;
            StartCoroutine(PursueAndAttackTarget());
        }
    }

    private bool IsTargetDead()
    {
        return monsterStats.IsDead;
    }

    public void SetDestination(Vector3 destination)
    {
        OnNewCommand?.Invoke();
        if (unitStats.IsDead == true) { return; }
        StopAllCoroutines();
        attackTarget = null;
        unitNavAgent.isStopped = false;
        unitNavAgent.destination = destination;
    }

    public void GetAttackTarget(GameObject target)
    {
        if (unitStats.IsDead == true) { return; }
        unitBaseAttack = unitStats.GetBaseAttack();
        attackRate = unitBaseAttack.attackRate;

        if (unitBaseAttack == null) { return; }

        StopAllCoroutines();
        unitNavAgent.isStopped = false;
        attackTarget = target;
        monsterStats = attackTarget.GetComponent<CharacterStats>();
        StartCoroutine(PursueAndAttackTarget());
    }

    private IEnumerator PursueAndAttackTarget()
    {
        if (unitStats.IsDead == true) { yield break; }
        if (attackTarget == null) { yield break; };

        unitNavAgent.isStopped = false;
        unitBaseAttack = unitStats.GetBaseAttack();

        bool isInAttackRange = (attackTarget.transform.position - transform.position).sqrMagnitude <
                ChaseRange * ChaseRange;

        while (IsOutOfWeaponRange(unitBaseAttack))
        {
            unitNavAgent.destination = attackTarget.transform.position;
            yield return null;
        }
        if (monsterStats.IsDead == true) { yield break; }
        unitNavAgent.isStopped = true;
        transform.LookAt(attackTarget.transform);
        OnUnitAttack?.Invoke();
    }

    private bool IsOutOfWeaponRange(AttackDefinition currentAttack)
    {
        return (attackTarget.transform.position - transform.position).sqrMagnitude >
            currentAttack.range * currentAttack.range;
    }

    private bool IsInChaseRange()
    {
        return (attackTarget.transform.position - transform.position).sqrMagnitude <
            ChaseRange * ChaseRange;
    }

    public void Hit()
    {
        if (attackTarget == null) { return; }
        if (unitBaseAttack is Weapon)
        {
            ((Weapon)unitStats.GetBaseAttack()).ExecuteAttack(gameObject, attackTarget);
            attackTarget.TryGetComponent<BloodEffects>(out BloodEffects bloodEffects);
            bloodEffects.BloodSplatterFromAttackAgainstMe(attackImpactTransform);
        }
        else if (unitBaseAttack is Spell)
        {
            ((Spell)unitBaseAttack).Cast
                (gameObject, unitStats.GetSpellSpawnPoint().position,
                attackTarget.GetComponent<CharacterStats>().GetAimAtPoint().position,
                layerIndexForMySpells);
            OnSpellCast?.Invoke();
        }
    }

    private void HandleDeath()
    {
        StopAllCoroutines();
        unitNavAgent.isStopped = true;
        StartCoroutine(DestroyUnit());
    }

    IEnumerator DestroyUnit()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    public void Select()
    {
        onSelected?.Invoke();
    }

    public void Deselect()
    {
        onDeselected?.Invoke();
    }
}
