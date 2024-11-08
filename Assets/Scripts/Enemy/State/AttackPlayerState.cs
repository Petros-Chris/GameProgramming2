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
        if (aiController.ally == null)
        {
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
            return;
        }

        // Debug.Log(aiController.player.position);
        // If person is not visible in attack range
        if (!aiController.CanSeePlayer(aiController.AttackRange))//IsEnemyInRange(aiController.AttackRange))
        {
            aiController.StateMachine.TransitionToState(StateType.Chase);
            return;
        }

        //? Maybe have a way to have the enemy turn slower so its possible to have the player dash out of the enemy fov, causing them to lose the player
        aiController.transform.LookAt(aiController.ally);
        aiController.Attack();

    }

    public void Exit()
    {
        //aiController.Animator.SetBool("isAttacking", false);
        aiController.Agent.isStopped = false;
    }
}

