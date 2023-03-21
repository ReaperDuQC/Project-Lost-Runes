using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/TargetKilled")]
public class KilledTargetDecision : Decision
{
    public override bool Decide(MonsterStateController monsterController)
    {
        bool targetIsDead = DeadCheck(monsterController);
        return targetIsDead;
    }

    public bool DeadCheck(MonsterStateController monsterController)
    {
        return monsterController.IsTargetDead();
    }
}
