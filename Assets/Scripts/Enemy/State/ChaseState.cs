using UnityEngine;
public class ChaseState : IState
{
    private EnemyAI aiController;

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
        // If person does not exist
        if (aiController.ally == null)
        {
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
            return;
        }

        aiController.LookAt(aiController.ally);

        aiController.ally = aiController.GetClosestEnemy();

        // If can not see person within sight
        if (!aiController.CanSeeEnemy(aiController.SightRange))//IsEnemyInRange(aiController.SightRange))
        {
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
            return;
        }

        // If can see person within attack
        if (aiController.CanSeeEnemy(aiController.AttackRange))//IsEnemyInRange(aiController.AttackRange))
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

