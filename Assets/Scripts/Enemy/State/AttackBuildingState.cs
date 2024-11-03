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
        if (aiController.CanSeePlayerWhileAttacking())
        {
            Debug.Log("OH THERE YOU ARE");
            aiController.StateMachine.TransitionToState(StateType.AttackPlayer);
        }

        aiController.transform.LookAt(aiController.building);
        if (aiController.CanSeeBuilding())
        {
            aiController.Attack();
        }
        else
        {
            aiController.StateMachine.TransitionToState(StateType.Patrol);
        }

    }

    public void Exit()
    {
        aiController.Agent.isStopped = false;
    }
}
