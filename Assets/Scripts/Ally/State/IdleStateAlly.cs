using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateAlly : IStateAlly
{
    private AllyAI aiController;
    private float idleDuration = 2f;
    private float idleTimer;

    public StateTypeAlly Type => StateTypeAlly.Idle;

    public IdleStateAlly(AllyAI aiController)
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
            Debug.Log("Ally: Patrolling!");
            aiController.StateMachine.TransitionToState(StateTypeAlly.Patrol);
        }
    }

    public void Exit()
    {
        // Cleanup if necessary
    }
}
