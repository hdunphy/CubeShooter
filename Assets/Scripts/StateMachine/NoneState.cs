using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneState : BaseState
{
    public NoneState(EnemyController controller) : base(controller)
    {
    }

    public override StateType GetStateType() => StateType.None;

    public override StateType Tick()
    {
        return StateType.None;
    }
}
