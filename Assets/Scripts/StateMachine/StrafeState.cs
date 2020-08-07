using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeState : BaseState
{
    public StrafeState(EnemyController controller) : base(controller) { }

    public override StateType GetStateType() => StateType.Strafe;

    public override StateType Tick()
    {
        StateType stateType = GetStateType();

        Vector3 targetPosition = controller.TargetedPlayer.transform.position;
        Vector3 offsetPlayer = transform.position - targetPosition;
        Vector3 dir = Vector3.Cross(offsetPlayer, Vector3.up);
        controller.TargetDestination = transform.position + dir;
        //var lookPos = targetPosition - transform.position;
        //lookPos.y = 0;
        //var rotation = Quaternion.
        if (Mathf.Abs(offsetPlayer.magnitude) > controller.ChaseDistance)
            stateType = StateType.Search;
        return stateType;
    }
}
