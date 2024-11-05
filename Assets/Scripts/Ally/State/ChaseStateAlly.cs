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
        Debug.Log("Ally: Chasing Enemy!");
    }

    public void Execute()
    {
        // Enemy doesn't exist
        if (aiController.enemy == null)
        {
            Debug.Log("Ally: Enemy Gone!");
            aiController.StateMachine.TransitionToState(StateTypeAlly.Patrol);
        }

        // Enemy is in attack range
        if (aiController.IsEnemyInRange(aiController.AttackRange))
        {
            Debug.Log("Ally: Engaging In Combat!");
            aiController.StateMachine.TransitionToState(StateTypeAlly.AttackEnemy);
        }
        aiController.Agent.destination = aiController.enemy.position;
    }

    public void Exit()
    {
        // No cleanup necessary
    }
}

