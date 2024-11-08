using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TowerAttack : AllyAI
{
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        StateMachine = new StateMachineAlly();
        StateMachine.AddState(new AttackEnemyTowerState(this));

        StateMachine.TransitionToState(StateTypeAlly.AttackEnemyFromTower);
    }

    void Update()
    {
        StateMachine.Update();
    }
}
