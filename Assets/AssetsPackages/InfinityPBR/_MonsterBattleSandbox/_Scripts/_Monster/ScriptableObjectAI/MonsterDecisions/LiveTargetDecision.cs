using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/LiveTargetActiveState")]
public class LiveTargetDecision : Decision
{
   public override bool Decide(MonsterStateController monsterController)
    {
        return ChaseTargetIsActiveAndAlive(monsterController);
    }

    private bool ChaseTargetIsActiveAndAlive(MonsterStateController monsterController)
    {
        if (monsterController.ChaseTarget == null) { return false; }
        if (monsterController.ChaseTarget.gameObject.activeSelf 
            && !monsterController.IsTargetDead())
        {
            return true;
        }
        return false;
    }
}
