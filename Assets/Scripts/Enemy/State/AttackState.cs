using UnityEngine;

public class AttackState : IState
{
    private EnemyAI aiController;

    public StateType Type => StateType.Attack;

    public AttackState(EnemyAI aiController)
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
        if (aiController.player != null)
        {
            if (Vector3.Distance(aiController.transform.position, aiController.player.position) > aiController.AttackRange)
            {
                // If the player moves away, transition back to ChaseState
                aiController.StateMachine.TransitionToState(StateType.Chase);
                return;
            }
        }
        else
        {
            aiController.StateMachine.TransitionToState(StateType.Patrol);
        }
        aiController.Attack();

    }

    public void Exit()
    {
        //aiController.Animator.SetBool("isAttacking", false);
        aiController.Agent.isStopped = false; // Resume the AI agent movement
    }
}

