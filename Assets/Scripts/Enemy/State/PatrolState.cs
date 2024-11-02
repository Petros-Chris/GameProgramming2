using UnityEngine;

public class PatrolState : IState
{
    private EnemyAI aiController;

    public StateType Type => StateType.Patrol;

    public PatrolState(EnemyAI aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        HeadToFishKingdom();
    }

    public void Execute()
    {
        if (aiController.CanSeePlayer())
        {
            Debug.Log("I SEE YOU");
            aiController.StateMachine.TransitionToState(StateType.Chase);
            return;
        }
    }

    public void Exit()
    {
        // Cleanup if necessary
    }

    private void HeadToFishKingdom()
    {
        if (aiController.fishKingdom != null)
        {
            aiController.Agent.destination = aiController.fishKingdom.position;
        }
        //transform.LookAt(fishKingdom);
    }
}
