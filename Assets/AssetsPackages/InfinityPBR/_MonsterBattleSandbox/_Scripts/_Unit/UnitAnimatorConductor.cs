using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAnimatorConductor : MonoBehaviour
{
    public enum TransitionParameter
    {
        Death, isWalking, Locomotion, Attack1, GotHit, castStart, castStop, AnimationCancel
    }

    [SerializeField] private Animator anim = null;
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Unit playerUnit = null;
    private CharacterStats stats = null;

    void Start()
    {
        stats = GetComponent<CharacterStats>();
        stats.OnCharacterDeath += DeathAnimation;
        stats.OnTakeDamage += GotHitAnimation;
        playerUnit.OnUnitAttack += AttackAnimation;
        playerUnit.OnNewCommand += CancelAnimation;
    }

    private void Update()
    {
        if (stats.IsDead == true) { return; }
        if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            anim.SetFloat(TransitionParameter.Locomotion.ToString(), agent.velocity.magnitude);
        }
    }

    void OnDisable()
    {
        stats.OnCharacterDeath -= DeathAnimation;
        stats.OnTakeDamage += GotHitAnimation;
        playerUnit.OnUnitAttack -= AttackAnimation;
        playerUnit.OnNewCommand -= CancelAnimation;
    }

    private void CancelAnimation()
    {
        if (stats.IsDead == true) { return; }
        anim.SetTrigger(TransitionParameter.AnimationCancel.ToString());
    }

    private void GotHitAnimation()
    {
        if (stats.IsDead == true) { return; }
        anim.SetTrigger(TransitionParameter.GotHit.ToString());
    }

    private void DeathAnimation()
    {
        anim.applyRootMotion = false;
        StopAllCoroutines();
        anim.SetTrigger(TransitionParameter.Death.ToString());
    }

    private void AttackAnimation()
    {
        if (stats.IsDead == true) { return; }
        if (stats.GetBaseAttack() is Weapon)
        {
            anim.SetTrigger(TransitionParameter.Attack1.ToString());
        }
        else if (stats.GetBaseAttack() is Spell)
        {
            anim.SetTrigger(TransitionParameter.castStart.ToString());
            StartCoroutine(CastSpellAnimation());
        }
    }

    IEnumerator CastSpellAnimation()
    {
        if (stats.IsDead == true) { yield break; }
        yield return new WaitForSeconds(stats.GetAttackRate()/2f);
        if (stats.IsDead == true) { yield break; }
        anim.SetTrigger(TransitionParameter.castStop.ToString());
    }
}
