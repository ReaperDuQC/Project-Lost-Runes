using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Scan")]
public class MonsterScanDecision : Decision
{
    public override bool Decide(MonsterStateController monsterController)
    {
        bool noEnemyInSight = Scan(monsterController);
        return noEnemyInSight;
    }

    private bool Scan(MonsterStateController monsterController)
    {
        monsterController.MonsterNavAgent.isStopped = true;
        monsterController.transform.Rotate(0, monsterController.SearchingTurnSpeed * Time.deltaTime, 0);
        return monsterController.CheckIfCountdownElapsed(monsterController.SearchDuration);
    }
}
