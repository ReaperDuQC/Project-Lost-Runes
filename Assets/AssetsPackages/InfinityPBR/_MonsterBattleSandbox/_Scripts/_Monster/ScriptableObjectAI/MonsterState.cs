using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/State")]
public class MonsterState : ScriptableObject
{
    public MonsterAction[] actions;
    public Transition[] transitions;
    public Color sceneGizmoColor = Color.grey;

    public void UpdateState(MonsterStateController monsterController)
    {
        DoMonsterActions(monsterController);
        CheckTransitions(monsterController);
    }

    private void DoMonsterActions(MonsterStateController monsterController)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].MonsterAct(monsterController);
        }
    }

    private void CheckTransitions(MonsterStateController monsterController)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            bool decisionSucceeded = transitions[i].decision.Decide(monsterController);

            if (decisionSucceeded)
            {
                monsterController.TransitionToState(transitions[i].trueState);
            }
            else
            {
                monsterController.TransitionToState(transitions[i].falseState);
            }
        }
    }
}
