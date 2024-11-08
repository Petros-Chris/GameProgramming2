using UnityEngine;

public class HeadToBuildingState : IState
{
    private EnemyAI aiController;

    public StateType Type => StateType.HeadToTower;

    public HeadToBuildingState(EnemyAI aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        if (aiController.building == null)
        {
            aiController.building = aiController.GetClosestBuilding();
        }
        aiController.Agent.destination = aiController.building.position;
    }

    public void Execute()
    {
        // I want it to change to who ever is closest, maybe not check as often
        aiController.ally = aiController.GetClosestEnemy();

        if (aiController.ally != null)
        {
            if (aiController.CanSeePlayer(aiController.SightRange))
            {
                aiController.StateMachine.TransitionToState(StateType.Chase);
                return;
            }
        }

        // Restarting the state, as the building got destroyed
        // before they made it to the building to switch to attack state,
        // Causing them to just stare at where the building used to be
        if (aiController.building == null)
        {
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
            return;
        }

        // CanSeeBuilding causing them to all walk forward dumbly 
        // ? Maybe there some way to lock their position so others don't push them out causing them to look for building again?
        if (aiController.IsBuildingInRange(aiController.AttackRange)) //&& aiController.CanSeeBuilding())
        {
            aiController.StateMachine.TransitionToState(StateType.AttackBuilding);
        }
    }

    public void Exit()
    {
        // Cleanup if necessary
    }
}
