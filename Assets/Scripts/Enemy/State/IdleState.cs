using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private EnemyAI aiController;
    private float idleDuration = 2f;
    private float idleTimer;

    public StateType Type => StateType.Idle;

    public IdleState(EnemyAI aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        idleTimer = 0f;
        //aiController.Animator.SetBool("isMoving", false);
    }

    public void Execute()
    {
        idleTimer += Time.deltaTime;
        //* After two seconds of doing nothing, it will switch to patrol mode
        if (idleTimer >= idleDuration)
        {
            aiController.StateMachine.TransitionToState(StateType.Patrol);
        }
    }

    public void Exit()
    {
        // Cleanup if necessary
    }
}
