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
    }

    public void Execute()
    {
        aiController.ally = aiController.GetClosestEnemy();
        
        if (aiController.ally == null)
        {
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
            return;
        }

        // Debug.Log(aiController.player.position);
        // If person is not visible in attack range
        if (!aiController.CanSeeEnemy(aiController.AttackRange))//IsEnemyInRange(aiController.AttackRange))
        {
            aiController.StateMachine.TransitionToState(StateType.Chase);
            return;
        }

        aiController.LookAt(aiController.ally);
        aiController.Attack();
    }

    public void Exit()
    {
        //aiController.Animator.SetBool("isAttacking", false);
        aiController.Agent.isStopped = false;
    }
}

