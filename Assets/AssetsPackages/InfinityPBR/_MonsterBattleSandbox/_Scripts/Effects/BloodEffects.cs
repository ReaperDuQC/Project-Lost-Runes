using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffects : MonoBehaviour
{
    // Might be useful for death event?
    CharacterStats stats;
    [SerializeField] ParticleSystem wasHitBloodParticleSystem = null;
    [SerializeField] ParticleSystem attackBloodParticleSystem = null;

    private void Start()
    {
        stats = GetComponent<CharacterStats>();
    }
    
    public void BloodSplatterFromAttackAgainstMe(Transform impactTransform)
    {
        wasHitBloodParticleSystem.gameObject.transform.rotation = impactTransform.rotation;
        wasHitBloodParticleSystem.gameObject.transform.position = impactTransform.position;
        wasHitBloodParticleSystem?.Play();
    }

    public void BloodSplatterWithMyAttack()
    {
        if (attackBloodParticleSystem == null) { return; }
        attackBloodParticleSystem?.Play();
    }
}