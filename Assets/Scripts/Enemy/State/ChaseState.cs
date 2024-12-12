using UnityEngine;
public class ChaseState : IState
{
    private EnemyAI aiController;

    Vector3 aaa;

    public StateType Type => StateType.Chase;

    public ChaseState(EnemyAI aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
    }

    public void Execute()
    {
        aiController.ally = aiController.GetClosestEnemy();

        // If person does not exist
        if (aiController.ally == null)
        {
            // If ai has reached its final destination
            if (aiController.Agent.remainingDistance <= aiController.Agent.stoppingDistance)
            {
                aiController.StateMachine.TransitionToState(StateType.HeadToTower);
                return;
            }
            // Loop to recheck
            return;
        }

        // If can not see person within sight
        if (!aiController.CanSeeEnemy(aiController.SightRange))
        {
            if (aiController.Agent.remainingDistance <= aiController.Agent.stoppingDistance)
            {
                aiController.StateMachine.TransitionToState(StateType.HeadToTower);
                return;
            }
            return;
        }

        aiController.LookAt(aiController.ally);

        // If can see person within attack
        if (aiController.CanSeeEnemy(aiController.AttackRange))
        {
            aiController.StateMachine.TransitionToState(StateType.AttackPlayer);
        }
        aiController.Agent.destination = aiController.ally.position;
    }

    public void Exit()
    {
        // No cleanup necessary
    }
}

