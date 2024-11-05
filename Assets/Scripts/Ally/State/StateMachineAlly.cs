using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineAlly
{
    private Dictionary<StateTypeAlly, IStateAlly> states = new Dictionary<StateTypeAlly, IStateAlly>();
    private IStateAlly currentState;

    public StateTypeAlly GetCurrentStateType()
    {
        return currentState.Type;
    }

    public void AddState(IStateAlly state)
    {
        if (!states.ContainsKey(state.Type))
        {
            states.Add(state.Type, state);
        }
    }

    public void TransitionToState(StateTypeAlly type)
    {
        currentState?.Exit();
        currentState = states[type];
        currentState.Enter();
    }

    public void Update()
    {
        //? I believe the ? is null check
        currentState?.Execute();
        //? Not 100% on what execute does
        //? Maybe Saves the states after they have been added
    }
}



