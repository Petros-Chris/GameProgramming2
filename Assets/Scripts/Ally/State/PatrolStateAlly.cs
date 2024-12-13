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
        aiController.enemy = aiController.GetClosestEnemy(aiController.SightRange);

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
        Vector3 placeToProtect = aiController.fishKingdom.transform.position;
        float protectxPos = placeToProtect.x;
        float protectzPos = placeToProtect.z;

        Vector3 point = new Vector3(Random.Range(protectxPos - 20, protectxPos + 20), 1, Random.Range(protectzPos - 20, protectzPos + 20));

        if (aiController.IsSpotReachable(point))
        {
            aiController.Agent.destination = point;
        }
    }
}

