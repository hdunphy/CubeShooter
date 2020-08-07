using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : BaseState
{

    public SearchState(EnemyController controller) : base(controller) { }
    public override StateType GetStateType() => StateType.Search;

    public override StateType Tick()
    {
        Vector3 targetPosition = controller.TargetedPlayer.transform.position;
        Vector3 offsetPlayer = transform.position - targetPosition;
        float distance = Vector3.Distance(transform.position, targetPosition);
        controller.TargetDestination = targetPosition;

        if (controller._debug)
            Debug.Log(distance);

        if (controller.NearestBullet != null)
            return StateType.AvoidBullet;
        else if (Mathf.Abs(distance) < controller.StrafeDistance)
            return StateType.Strafe;
        else
            return GetStateType();
    }
}
