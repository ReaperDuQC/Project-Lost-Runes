using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ActiveState")]
public class ActiveStateDecision : Decision
{
    public override bool Decide(MonsterStateController monsterController)
    {
        bool chaseTargetIsActive = monsterController.ChaseTarget.gameObject.activeSelf;
        return chaseTargetIsActive;
    }
}