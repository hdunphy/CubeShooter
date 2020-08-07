using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFactory
{
    public static BaseState CreateBaseState(StateType stateType, EnemyController controller)
    {
        BaseState returnState = null;
        switch (stateType)
        {
            case StateType.AvoidBullet:
                returnState = new AvoidBulletState(controller);
                break;
            case StateType.Search:
                returnState = new SearchState(controller);
                break;
            case StateType.Strafe:
                returnState = new StrafeState(controller);
                break;
            case StateType.None:
                returnState = new NoneState(controller);
                break;
        }
        return returnState;
    }
}
