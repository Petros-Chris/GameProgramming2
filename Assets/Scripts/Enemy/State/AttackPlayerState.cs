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
        aiController.Agent.isStopped = true; // Stop the AI agent movement
    }

    public void Execute()
    {
        // Check if the player is within attack range
        if (!aiController.CanSeePlayerWhileAttacking())
        {
            Debug.Log("GET BACK HERE");
            aiController.StateMachine.TransitionToState(StateType.Chase);
        }
        //? Maybe have a way to have the enemy turn slower so its possible to have the player dash out of the enemy fov, causing them to lose the player

        //Perhaps i could change look based on raycast data (player, ally, etc)
        aiController.transform.LookAt(aiController.player);
        aiController.Attack();

    }

    public void Exit()
    {
        //aiController.Animator.SetBool("isAttacking", false);
        aiController.Agent.isStopped = false; // Resume the AI agent movement
    }
}

