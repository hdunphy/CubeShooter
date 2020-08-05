using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StateType { Search, AvoidBullet, Strafe, None }

public class StateMachine
{

    private Dictionary<StateType, BaseState> _availableStates;
    public BaseState CurrentState { get; private set; }
    public event Action<BaseState> OnStateChange;

    public StateMachine(Dictionary<StateType, BaseState> states)
    {
        _availableStates = states;
    }

    // Update is called once per frame
    public void Update()
    {
        if(CurrentState == null)
        {
            CurrentState = _availableStates.Values.First();
        }

        StateType? nextState = CurrentState?.Tick();

        if(nextState != null && nextState != CurrentState?.GetStateType())
        {
            SwitchOnNextState(nextState.Value);
        }
    }

    private void SwitchOnNextState(StateType nextState)
    {
        CurrentState = _availableStates[nextState];
        OnStateChange?.Invoke(CurrentState);
    }
}
