using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Look")]
public class LookDecision : Decision
{
    public override bool Decide(MonsterStateController monsterController)
    {
        bool targetVisible = Look(monsterController);
        return targetVisible;
    }

    private bool Look(MonsterStateController monsterController)
    {
        Debug.DrawRay(monsterController.EyesTransform.position,
           monsterController.EyesTransform.forward.normalized * monsterController.LookSphereCastRange, Color.green);

        if (monsterController.ChaseTarget != null && monsterController.IsTargetDead())
        {
            return IsThereACloseTargetStillAlive(monsterController);
        }

        RaycastHit hit;

        if (Physics.SphereCast
            (monsterController.EyesTransform.position, 
            monsterController.LookSphereCastRadius,
            monsterController.EyesTransform.forward, out hit, monsterController.LookSphereCastRange)
            && hit.collider.CompareTag("Player"))
        {
            monsterController.ChaseTarget = hit.transform;
            return true;
        }
        else
        {
            monsterController.ChaseTarget = null;
            return false;
        }
    }

    private bool IsThereACloseTargetStillAlive(MonsterStateController monsterController)
    {
        Debug.Log("OverlapSphere!--Any targets still alive?");
        bool maybeTheyAreAllDead = true;
        Collider[] nearbyCollidersToCheck =
        Physics.OverlapSphere(monsterController.gameObject.transform.position, monsterController.CloseProximityAwarenessRadius);
        foreach (var otherCollider in nearbyCollidersToCheck)
        {
            if (otherCollider.CompareTag("Player"))
            {
                monsterController.ChaseTarget = otherCollider.transform;
                if (!monsterController.IsTargetDead())
                {
                    Debug.Log("New Chase Target");
                    maybeTheyAreAllDead = false;
                    return true;
                }
                else if (monsterController.IsTargetDead())
                {
                    monsterController.ChaseTarget = null;
                    maybeTheyAreAllDead = true;
                    continue;
                }
            }
        }
        return !maybeTheyAreAllDead;
    }
}
