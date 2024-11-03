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
        // Appartely it's possible to get null here
        if (aiController.building == null)
        {
            aiController.building = aiController.GetClosestBuilding();
        }
        aiController.Agent.destination = aiController.building.position;
        Debug.Log("HIYA FROM ENTER IN PATROL");

    }

    public void Execute()
    {
        if (aiController.CanSeePlayer())
        {
            Debug.Log("I SEE YOU");
            aiController.StateMachine.TransitionToState(StateType.Chase);
            return;
        }
        if (aiController.IsBuildingInAttackRange())
        {
            Debug.Log("TIME TO DESTROY >:)");
            aiController.StateMachine.TransitionToState(StateType.AttackBuilding);
            return;
        }
        // Restarting the state, as the building got destroyed
        // before they made it to the building to switch to attack state,
        // Causing them to just stare at where the building used to be
        if (aiController.building == null)
        {
            aiController.StateMachine.TransitionToState(StateType.Patrol);
        }
    }

    public void Exit()
    {
        // Cleanup if necessary
    }
}
