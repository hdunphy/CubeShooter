using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : BaseState
{

    public SearchState(EnemyController controller) : base(controller) { }
    public override StateType GetStateType() => StateType.Search;

    public override StateType Tick()
    {
        controller.TargetDestination = controller.TargetedPlayer.transform.position;

        if (controller.NearestBullet != null)
            return StateType.AvoidBullet;
        else
            return GetStateType();
    }
}
