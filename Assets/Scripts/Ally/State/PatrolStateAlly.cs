using UnityEngine;

public class PatrolStateAlly : IStateAlly
{
    private AllyAI aiController;

    public StateTypeAlly Type => StateTypeAlly.Patrol;

    public PatrolStateAlly(AllyAI aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {

        aiController.enemy = aiController.GetClosestEnemy();
    }

    public void Execute()
    {
        Debug.Log("Ally: Going Chase!");
        aiController.StateMachine.TransitionToState(StateTypeAlly.Chase);

    }

    public void Exit()
    {
        // Cleanup if necessary
    }
}
