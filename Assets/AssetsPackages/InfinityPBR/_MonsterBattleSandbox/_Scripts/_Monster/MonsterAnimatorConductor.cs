using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAnimatorConductor : MonoBehaviour
{
    public enum TransitionParameter
    {
        Death, Locomotion, Attack1, SpecialAttack
    }

    [SerializeField] private Animator anim = null;
    [SerializeField] private NavMeshAgent agent = null;
    private MonsterAttack monsterAttack;
    private MonsterSpecialAttack monsterSpecialAttack;
    private CharacterStats monsterStats;
    private float locomotionSpeed;
    private Rigidbody monsterRigidbody = null;
    private string defaultStateClipName;
    private AnimatorClipInfo[] currentClipInfo;

    void Start()
    {
        monsterStats = GetComponent<CharacterStats>();
        monsterAttack = GetComponent<MonsterAttack>();
        monsterSpecialAttack = GetComponent<MonsterSpecialAttack>();
        TryGetComponent<Rigidbody>(out monsterRigidbody);

        if (monsterAttack != null)
        {
            monsterAttack.OnBaseAttack += BaseAttackAnimation;
        }
        if (monsterSpecialAttack != null)
        {
            monsterSpecialAttack.OnSpecialAttack += SpecialAttackAnimation;
        }

        monsterStats.OnCharacterDeath += DeathAnimation;
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
    }

    void OnDisable()
    {
        if (monsterAttack != null)
        {
            monsterAttack.OnBaseAttack -= BaseAttackAnimation;
        }
        if (monsterSpecialAttack != null)
        {
            monsterSpecialAttack.OnSpecialAttack -= SpecialAttackAnimation;
        }

        monsterStats.OnCharacterDeath -= DeathAnimation;
    }

    private void Update()
    {
        if (monsterStats.IsDead == true) return;
        locomotionSpeed = Mathf.Lerp(locomotionSpeed, agent.velocity.magnitude, Time.deltaTime * 10);
        if (agent.velocity.magnitude > 0)
            anim.SetFloat(TransitionParameter.Locomotion.ToString(), locomotionSpeed);
    }

    private void DeathAnimation()
    {
        anim.applyRootMotion = false;
        anim.SetTrigger(TransitionParameter.Death.ToString());
    }

    private void BaseAttackAnimation()
    {
        if (monsterStats.IsDead == true) return;

        if (CanAttackPlay())
        {
            anim.SetTrigger(TransitionParameter.Attack1.ToString());
        }
    }

    private void SpecialAttackAnimation()
    {
        anim.SetTrigger
            (TransitionParameter.SpecialAttack.ToString());
    }

    private bool CanAttackPlay()
    {
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        if (defaultStateClipName == currentClipInfo[0].clip.name)
        {
            return true;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime
            > .9f) // && !anim.IsInTransition(0))
        {
            return true;
        }
        return false;
    }
}
