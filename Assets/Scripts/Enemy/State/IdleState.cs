using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private EnemyAI aiController;
    private float idleDuration = 0.2f;
    private float idleTimer;

    public StateType Type => StateType.Idle;

    public IdleState(EnemyAI aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        idleTimer = 0f;
    }

    public void Execute()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            aiController.healthBarScript.UpdateHealthBar(aiController.health, aiController.maxHealth);
            aiController.StateMachine.TransitionToState(StateType.HeadToTower);
        }
    }

    public void Exit()
    {
        // Cleanup if necessary
    }
}
