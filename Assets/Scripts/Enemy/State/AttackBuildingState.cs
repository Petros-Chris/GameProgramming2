using UnityEngine;

public class AttackBuildingState : IState
{

    //! Player dying infront of enemy when enemy is next to building causes freak out
    private EnemyAI aiController;

    public StateType Type => StateType.AttackBuilding;

    public AttackBuildingState(EnemyAI aiController)
    {
        this.aiController = aiController;
    }
    public void Enter()
    {
        aiController.Agent.isStopped = true;
    }

    public void Execute()
    {
        if (aiController.ally != null)
        {
            if (aiController.IsEnemyInRange(aiController.SightRange))
            {
                Debug.Log("OH THERE YOU ARE");
                aiController.StateMachine.TransitionToState(StateType.Chase);
                return;
            }
        }
        
        if (aiController.building == null)
        {
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
            return; // This stops the current execute in its path
        }
        aiController.transform.LookAt(aiController.building);

        // Makes them not shoot if theres something in the way
        if (aiController.CanSeeBuilding())
        {
            aiController.Attack();
            return;
        }

        // A lot of enemies will cause each other to cycle between
        // patrol and attack building constantly, affecting fps a bit 
        // Even so, it seems to change a looot, like 30 enemies caused at least 10k of switching in a few seconds
        if (!aiController.IsBuildingInRange(aiController.AttackRange))//!aiController.CanSeeBuilding())
        {
            // Its seeing and unseeing constantly for some reason
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
        }
    }

    public void Exit()
    {
        aiController.Agent.isStopped = false;
    }
}
