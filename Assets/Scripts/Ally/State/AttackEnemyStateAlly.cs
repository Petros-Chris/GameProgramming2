using UnityEngine;

public class AttackEnemyState : IStateAlly
{
    private AllyAI aiController;

    public StateTypeAlly Type => StateTypeAlly.AttackEnemy;

    public AttackEnemyState(AllyAI aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        aiController.Agent.isStopped = true;
    }

    public void Execute()
    {
        // Enemy doesn't exist
        if (aiController.enemy != null)
        {
            // Enemy is not in attack range
            if (!aiController.CanSeeEnemy(aiController.AttackRange))
            {
                aiController.StateMachine.TransitionToState(StateTypeAlly.Chase);
                return;
            }
            aiController.LookAt(aiController.enemy);
            aiController.Attack();
        }
        else
        {
            aiController.StateMachine.TransitionToState(StateTypeAlly.Patrol);
        }


    }

    public void Exit()
    {
        aiController.Agent.isStopped = false;
    }
}

