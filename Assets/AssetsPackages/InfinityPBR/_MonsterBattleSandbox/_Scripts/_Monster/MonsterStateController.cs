using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* ---About MonsterStateController---
    
    This script is from "Pluggable AI With Scriptable Objects"
    
    https://learn.unity.com/tutorial/pluggable-ai-with-scriptable-objects

    It controls monster behavior with state machines.

    It gets the attackRange and attackCooldown from CharacterStats.
    
    CurrentState is the starting state.

 */

public class MonsterStateController : MonoBehaviour, IAttackable 
{
    public MonsterState CurrentState;

    [HideInInspector] public CharacterStats TargetStats;
    private CharacterStats monsterStats;
    public Transform EyesTransform;

    // State for remaining in current state (not transitioning)
    public MonsterState RemainState;

    public float LookSphereCastRange = 5f;
    public float LookSphereCastRadius = 3f;
    public float CloseProximityAwarenessRadius = 3f;
    public float AttackForce;

    [HideInInspector] public float AttackCooldown;
    [HideInInspector] public float AttackRange;
    [HideInInspector] public float SpecialAttackRange;
    [HideInInspector] public float SpecialAttackCooldown;
    [HideInInspector] public float SearchDuration;
    [HideInInspector] public float SearchingTurnSpeed;

    [HideInInspector] public List<Transform> waypointList;

    [HideInInspector] public NavMeshAgent MonsterNavAgent;
    [HideInInspector] public MonsterAttack MonsterAttack;
    [HideInInspector] public MonsterSpecialAttack MonsterSpecialAttack;
    [HideInInspector] public int NextWaypoint;
    [HideInInspector] public Transform ChaseTarget;
    [HideInInspector] public float StateTimeElapsed;
    [HideInInspector] public bool wasAttacked;

    public static Action<MonsterStateController> OnMonsterSpawn;
    public Action<MonsterState> OnMonsterStateChange;

    void Awake()
    {
        waypointList = null;
        StateTimeElapsed = 0;
        MonsterAttack = GetComponent<MonsterAttack>();
        MonsterSpecialAttack = GetComponent<MonsterSpecialAttack>();
        MonsterNavAgent = GetComponent<NavMeshAgent>();
        monsterStats = GetComponent<CharacterStats>();
        monsterStats.OnCharacterDeath += MonsterDead;
    }

    private void OnDisable()
    {
        monsterStats.OnCharacterDeath -= MonsterDead;
    }

    public void Start()
    {
        AttackDefinition BaseAttack = monsterStats.GetBaseAttack();
        if (BaseAttack != null)
        {
            AttackRange = monsterStats.GetAttackRange();
            AttackCooldown = monsterStats.GetAttackRate();
        }
        AttackDefinition SpecialAttack = monsterStats.GetSpecialAttack();
        if (SpecialAttack != null)
        {
            SpecialAttackRange = monsterStats.GetSpecialAttackRange();
            SpecialAttackCooldown = monsterStats.GetSpecialAttackRate();
        }
        MonsterNavAgent.enabled = false;
        OnMonsterSpawn?.Invoke(this);
        MonsterNavAgent.enabled = true;
        MonsterNavAgent.speed = monsterStats.GetNavAgentSpeed();
    }

    public void SetWaypoints(List<Transform> waypoints)
    {
        waypointList = waypoints;
    }

    void Update()
    {
        if (waypointList == null) { return; }
        if (monsterStats.IsDead) { return; }
        CurrentState.UpdateState(this);
    }

    private void MonsterDead()
    {
        MonsterNavAgent.isStopped = true;
        monsterStats.GetRigidbody().isKinematic = true;
        StartCoroutine(DestroyMonster());
    }

    IEnumerator DestroyMonster()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    public void LookAtTarget()
    {
        if (ChaseTarget != null)
        {
            transform.LookAt(ChaseTarget.transform);
        }
    }

    public bool IsTargetDead()
    {
        if (ChaseTarget == null) { return true; }
        TargetStats = ChaseTarget.GetComponent<CharacterStats>();
        bool isTargetDead = TargetStats.IsDead;
        return isTargetDead;
    }

    void OnDrawGizmos()
    {
        if (CurrentState != null && EyesTransform != null)
        {
            Gizmos.color = CurrentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(EyesTransform.position, LookSphereCastRadius);
        }
    }

    public void TransitionToState(MonsterState nextState)
    {
        if (nextState != RemainState)
        {
            CurrentState = nextState;
            OnExitState();
        }
    }

    public bool CheckIfCountdownElapsed(float duration)
    {
        StateTimeElapsed += Time.deltaTime;
        return (StateTimeElapsed >= duration);
    }

    private void OnExitState()
    {
        StateTimeElapsed = 0;
        OnMonsterStateChange?.Invoke(CurrentState);
    }

    public void OnAttack(GameObject attacker, Attack attack)
    {
        wasAttacked = true;
    }
}
