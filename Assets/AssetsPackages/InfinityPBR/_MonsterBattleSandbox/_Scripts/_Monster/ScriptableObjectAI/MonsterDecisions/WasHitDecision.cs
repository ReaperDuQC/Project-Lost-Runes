using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/WasHitDecision")]
public class WasHitDecision : Decision
{
    public override bool Decide(MonsterStateController monsterController)
    {
        return WasHit(monsterController);
    }

    public bool WasHit(MonsterStateController monsterController)
    {
        return monsterController.wasAttacked;
    }
}
