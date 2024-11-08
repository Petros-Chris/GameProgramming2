using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeAlly : AllyAI
{
    // Was a test
    // Start is called before the first frame update
    void Start()
    {
        StateMachine = new StateMachineAlly();
        StateMachine.AddState(new PatrolStateAlly(this));

        StateMachine.TransitionToState(StateTypeAlly.Patrol);
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.Update();
    }
}
