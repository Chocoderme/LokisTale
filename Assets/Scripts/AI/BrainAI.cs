using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public enum ACTION_TYPE
{
    MOVEMENT,
    ATTACK,
    DIALOG
}

public enum ACTION_AI
{
    MOVE_TOWARD,
    RUN_AWAY,
    SHOOT_TO_PLAYER,
    ATTACK_PLAYER,
    HIDE,
    DO_NOTHING
}

[Serializable]
class ActionAIDecision
{
    public ACTION_AI Action;
    public ACTION_TYPE Type;
    public int Priority;
}

public class BrainAI : MonoBehaviour {

    [SerializeField] ActionAIDecision[] Actions;

    [SerializeField] IEnumerable<ActionAIDecision> listedPriorityAction;

    List<ACTION_AI> ActionsToDo = new List<ACTION_AI>();

    PlayerSensor mPlayerSensor;
    EnemySensor mSelfSensor;

    void Awake()
    {
        listedPriorityAction = Actions.OrderBy(action => action.Priority);

        foreach (ActionAIDecision action in listedPriorityAction)
        {
            Debug.Log(action.Priority);
        }
    }

    bool CanDoAction(ACTION_AI actionType)
    {
        bool result = false;

        switch (actionType)
        {
            case ACTION_AI.MOVE_TOWARD:
                result = mPlayerSensor.CanMoveTowardPlayer();
                break;
            case ACTION_AI.RUN_AWAY:
                result = mPlayerSensor.CanRunAwayFromPlayer();
                break;
            case ACTION_AI.HIDE:
                result = mPlayerSensor.CanHideFromPlayer();
                break;
            case ACTION_AI.SHOOT_TO_PLAYER:
                result = mSelfSensor.CanShoot();
                break;
            case ACTION_AI.ATTACK_PLAYER:
                result = mSelfSensor.CanAttack();
                break;
        }

        return result;
    }

    void DoActionType(ACTION_AI actionType)
    {
        switch (actionType)
        {
            case ACTION_AI.MOVE_TOWARD:
                Debug.Log("[AI] Move to player");
                break;
            case ACTION_AI.RUN_AWAY:
                Debug.Log("[AI] Run away from player");
                break;
            case ACTION_AI.HIDE:
                Debug.Log("[AI] Hide from player");
                break;
            case ACTION_AI.SHOOT_TO_PLAYER:
                Debug.Log("[AI] Shoot to player");
                break;
            case ACTION_AI.ATTACK_PLAYER:
                Debug.Log("[AI] Attack player");
                break;
        }
    }

    void ResolveBestPriorityAction()
    {
        List<ACTION_TYPE> ActionsCompatible = new List<ACTION_TYPE>();

        // Actions sorted by priority;
        foreach (ActionAIDecision action in listedPriorityAction)
        {
            if (CanDoAction(action.Action))
            {
                if (!ActionsCompatible.Contains(action.Type))
                {
                    ActionsCompatible.Add(action.Type);
                    ActionsToDo.Add(action.Action);
                }
            }
        }
    }

    void Think()
    {
        while (ActionsToDo.Count > 0)
            ActionsToDo.RemoveAt(0);

        ResolveBestPriorityAction();
    }

    void DoAction()
    {
        foreach (ACTION_AI action in ActionsToDo)
        {
            DoActionType(action);
        }
    }

}
