using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    protected EnemyController controller;
    protected Transform transform;

    public BaseState(EnemyController controller)
    {
        this.controller = controller;
        transform = controller.transform;
    }
    public abstract StateType Tick();

    public abstract StateType GetStateType();
}
