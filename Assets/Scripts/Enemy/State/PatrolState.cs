using UnityEngine;

public class PatrolState : IState
{
    private EnemyAI aiController;

    public StateType Type => StateType.HeadToTower;

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
        //Debug.Log("HIYA FROM ENTER IN PATROL");

    }

    public void Execute()
    {
        //aiController.transform.LookAt(aiController.building);
        if (aiController.player != null)
        {
            if (aiController.CanSeePlayer(aiController.SightRange))
            {
                Debug.Log("I SEE YOU");
                aiController.StateMachine.TransitionToState(StateType.Chase);
                return;
            }
        }
        // CanSeeBuilding causing them to all walk forward dumbly 
        // ? Maybe there some way to lock their position so others don't push them out causing them to look for building again?
        if (aiController.IsBuildingInAttackRange()) //&& aiController.CanSeeBuilding())
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
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
        }
    }

    public void Exit()
    {
        // Cleanup if necessary
    }
}
