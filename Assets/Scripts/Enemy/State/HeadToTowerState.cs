using UnityEngine;

public class HeadToTowerState : IState
{
    private EnemyAI aiController;

    public StateType Type => StateType.HeadToTower;

    public HeadToTowerState(EnemyAI aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        if (aiController.building != null)
        {
            if (!aiController.building.gameObject.activeInHierarchy)
            {
                aiController.building = aiController.GetClosestBuilding();
            }
        }
        else
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
            if (aiController.CanSeeEnemy(aiController.SightRange))
            {
                aiController.StateMachine.TransitionToState(StateType.Chase);
                return;
            }
        }

        // Restarting the state, as the building got destroyed
        // before they made it to the building to switch to attack state,
        // Causing them to just stare at where the building used to be

        // Checks if build was prem destroyed
        if (aiController.building == null)
        {
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
            return;
        }

        // Checks if build was disabled
        if (!aiController.building.gameObject.activeInHierarchy)
        {
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
            return;
        }

        if (aiController.IsBuildingInRange(aiController.AttackRange))
        {
            aiController.StateMachine.TransitionToState(StateType.AttackBuilding);
        }
    }

    public void Exit()
    {
        // Cleanup if necessary
    }
}
