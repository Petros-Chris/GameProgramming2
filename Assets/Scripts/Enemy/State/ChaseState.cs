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
        // No animations, so no need to set any animator parameters
    }

    public void Execute()
    {
        if (!aiController.CanSeePlayer())
        {
            Debug.Log("I don't see you anymore :<");
            aiController.StateMachine.TransitionToState(StateType.Patrol);
            return;
        }

        if (aiController.IsPlayerInAttackRange())
        {
            Debug.Log("DIE");
            aiController.StateMachine.TransitionToState(StateType.Attack);
            return;
        }

        aiController.Agent.destination = aiController.player.position;
    }

    public void Exit()
    {
        // No cleanup necessary
    }
}

