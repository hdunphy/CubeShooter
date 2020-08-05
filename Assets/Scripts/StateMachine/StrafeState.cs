using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeState : BaseState
{
    public StrafeState(EnemyController controller) : base(controller) { }

    public override StateType GetStateType() => StateType.Strafe;

    public override StateType Tick()
    {
        throw new System.NotImplementedException();
    }
}
