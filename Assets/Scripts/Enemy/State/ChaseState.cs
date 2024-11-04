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
        if (!aiController.IsPlayerInRange(aiController.SightRange) && aiController.CanSeePlayer(aiController.SightRange))
        {
            Debug.Log("Oh well guess it's time to attack fish kingdom");
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
            return;
        }

        if (aiController.IsPlayerInAttackRange() && aiController.CanSeePlayer(aiController.SightRange))
        {
            Debug.Log("DIE");
            aiController.StateMachine.TransitionToState(StateType.AttackPlayer);
            return;
        }

        aiController.Agent.destination = aiController.player.position;
    }

    public void Exit()
    {
        // No cleanup necessary
    }
}

