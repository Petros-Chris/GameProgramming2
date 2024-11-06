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

        aiController.transform.LookAt(aiController.ally);

        // Slow down?
        // I want it to change to who ever is closest
        aiController.ally = aiController.GetClosestEnemy();
        // If can not see person within sight
        if (!aiController.IsEnemyInRange(aiController.SightRange))
        {
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
            return;
        }

        // If can see person within attack
        if (aiController.IsEnemyInRange(aiController.AttackRange))
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

