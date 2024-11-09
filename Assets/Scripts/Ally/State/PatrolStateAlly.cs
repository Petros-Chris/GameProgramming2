using UnityEngine;
using System.Collections;

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
    }

    public void Execute()
    {
        // Somehow make this not run as often
        aiController.enemy = aiController.GetClosestEnemy();

        if (aiController.enemy != null)
        {
            // Enemy in sight range
            if (aiController.CanSeeEnemy(aiController.SightRange))
            {
                aiController.StateMachine.TransitionToState(StateTypeAlly.Chase);
                return;
            }
        }

        // Ally done current path
        if (!aiController.Agent.pathPending && aiController.Agent.remainingDistance <= aiController.Agent.stoppingDistance)
        {
            GenerateAndHeadToNewPoint();
        }
    }

    public void Exit()
    {
    }

    private void GenerateAndHeadToNewPoint()
    {
        Vector3 point = new Vector3(Random.Range(8, 17), 1, Random.Range(0, -12));
        aiController.Agent.destination = point;
    }
}

