using UnityEngine;
public class ChaseStateAlly : IStateAlly
{
    private AllyAI aiController;

    public StateTypeAlly Type => StateTypeAlly.Chase;

    public ChaseStateAlly(AllyAI aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        aiController.Agent.destination = aiController.enemy.position;
    }

    public void Execute()
    {
        aiController.enemy = aiController.GetClosestEnemy(aiController.SightRange);
        // Enemy doesn't exist
        if (aiController.enemy == null)
        {
            // If ai has reached its final destination
            if (aiController.Agent.remainingDistance <= aiController.Agent.stoppingDistance)
            {
                aiController.StateMachine.TransitionToState(StateTypeAlly.Patrol);
                return;
            }
            return;
        }

        // If can not see enemy within sight
        if (!aiController.CanSeeEnemy(aiController.SightRange))
        {
            if (aiController.Agent.remainingDistance <= aiController.Agent.stoppingDistance)
            {
                aiController.StateMachine.TransitionToState(StateTypeAlly.Patrol);
                return;
            }
            return;
        }

        // Enemy is in attack range
        if (aiController.CanSeeEnemy(aiController.AttackRange))
        {
            aiController.StateMachine.TransitionToState(StateTypeAlly.AttackEnemy);
            return;
        }

        aiController.Agent.destination = aiController.enemy.position;
    }

    public void Exit()
    {
        // No cleanup necessary
    }
}

