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
        if (aiController.enemy == null) 
        {
            aiController.StateMachine.TransitionToState(StateTypeAlly.Patrol);
        }

        // Enemy is not in attack range
        if (!aiController.IsEnemyInRange(aiController.AttackRange))
        {
            aiController.StateMachine.TransitionToState(StateTypeAlly.Chase);
        }
        aiController.transform.LookAt(aiController.enemy);
        aiController.Attack();
    }

    public void Exit()
    {
        aiController.Agent.isStopped = false;
    }
}

