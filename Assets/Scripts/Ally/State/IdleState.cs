using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateAlly : IStateAlly
{
    private AllyAI aiController;
    public StateTypeAlly Type => StateTypeAlly.Idle;

    public IdleStateAlly(AllyAI aiController)
    {
        this.aiController = aiController;
    }

    public void Enter() { }

    public void Execute()
    {
        if (aiController.GetClosestEnemy(aiController.AttackRange) != null)
        {
            aiController.StateMachine.TransitionToState(StateTypeAlly.AttackEnemyFromTower);
        }
    }

    public void Exit()
    {
    }
}
