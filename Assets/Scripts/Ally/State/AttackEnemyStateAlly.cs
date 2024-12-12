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
        // Get nearest enemy
        aiController.enemy = aiController.GetClosestEnemy(aiController.SightRange);
        // Enemy doesn't exist
        if (aiController.enemy == null)
        {
            aiController.StateMachine.TransitionToState(StateTypeAlly.Patrol);
            return;
        }
        // Enemy is not seen in attack range
        if (!aiController.CanSeeEnemy(aiController.AttackRange))
        {
            aiController.StateMachine.TransitionToState(StateTypeAlly.Chase);
            return;
        }
        // Look at and attack enemy
        aiController.LookAt(aiController.enemy);
        aiController.Attack(aiController.enemy);
    }

    public void Exit()
    {
        aiController.Agent.isStopped = false;
    }
}

