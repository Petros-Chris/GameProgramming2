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
        if (aiController.CanSeePlayer(aiController.AttackRange))
        {
            Debug.Log("OH THERE YOU ARE");
            aiController.StateMachine.TransitionToState(StateType.AttackPlayer);
        }

        // A lot of enemies will cause each other to cycle between
        // patrol and attack building constantly, affecting fps a bit 
        // Even so, it seems to change a looot, like 30 enemies caused at least 10k of switching in a few seconds
        //! Cause is lookat, its causing werid behavior

        aiController.transform.LookAt(aiController.building);
        aiController.Attack();

        if (!aiController.CanSeeBuilding())
        {
            // Its seeing and unseeing constantly for some reason
            aiController.StateMachine.TransitionToState(StateType.Patrol);
        }

    }

    public void Exit()
    {
        aiController.Agent.isStopped = false;
    }
}
