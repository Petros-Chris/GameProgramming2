using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class MeleeEnemy : EnemyAI
{

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        building = GetClosestBuilding();
        ally = GetClosestEnemy();

        StateMachine = new StateMachine();
        StateMachine.AddState(new HeadToTowerState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackBuildingState(this));
        StateMachine.AddState(new AttackPlayerState(this));

        StateMachine.TransitionToState(StateType.HeadToTower);
    }

    public void Update()
    {
        StateMachine.Update();
        currentState = StateMachine.GetCurrentStateType();
    }
}
