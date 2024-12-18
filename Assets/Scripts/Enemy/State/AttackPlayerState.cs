using UnityEngine;

public class AttackPlayerState : IState
{
    private EnemyAI aiController;


    public StateType Type => StateType.AttackPlayer;

    public AttackPlayerState(EnemyAI aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        //aiController.Animator.SetBool("isAttacking", true);
        aiController.Agent.isStopped = true;
        // aiController.thinkingSpeed = 0.01f;
    }

    public void Execute()
    {
        aiController.ally = aiController.GetClosestEnemy();

        if (aiController.ally == null)
        {
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
            return;
        }

        // If person is not visible in attack range
        if (!aiController.CanSeeEnemy(aiController.AttackRange))
        {
            aiController.StateMachine.TransitionToState(StateType.Chase);
            return;
        }

        aiController.LookAt(aiController.ally);
        aiController.Attack(aiController.ally);
    }

    public void Exit()
    {
        //aiController.Animator.SetBool("isAttacking", false);
        // aiController.thinkingSpeed = 0.5f;
        aiController.Agent.isStopped = false;
    }
}

