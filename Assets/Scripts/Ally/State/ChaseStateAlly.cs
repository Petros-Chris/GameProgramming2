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
    }

    public void Execute()
    {
        // Enemy doesn't exist
        if (aiController.enemy != null)
        {
            // Enemy is in attack range
            if (aiController.CanSeeEnemy(aiController.AttackRange))
            {
                aiController.StateMachine.TransitionToState(StateTypeAlly.AttackEnemy);
                return;
            }
            aiController.Agent.destination = aiController.enemy.position;
        }
        else
        {
            aiController.StateMachine.TransitionToState(StateTypeAlly.Patrol);
        }
    }

    public void Exit()
    {
        // No cleanup necessary
    }
}

