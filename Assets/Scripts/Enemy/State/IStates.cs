using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    StateType Type { get; }

    //? Start from what i understand
    void Enter();
    //? Update from what i understand
    void Execute();
    //? like onDestroy from what I understand
    void Exit();
}

