using UnityEngine;

public class AttackBuildingState : IState
{
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
        aiController.ally = aiController.GetClosestEnemy();
        
        if (aiController.ally != null)
        {
            if (aiController.CanSeeEnemy(aiController.SightRange))
            {
                aiController.StateMachine.TransitionToState(StateType.Chase);
                return;
            }
        }

        if (aiController.building == null)
        {
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
            return; // This stops the current execute in its path
        }

        aiController.LookAt(aiController.building);

        // Makes them not shoot if theres something in the way
        if (aiController.CanSeeBuilding())
        {
            aiController.Attack();
            return;
        }

        if (!aiController.IsBuildingInRange(aiController.AttackRange))//!aiController.CanSeeBuilding())
        {
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
        }
    }

    public void Exit()
    {
        aiController.Agent.isStopped = false;
    }
}
