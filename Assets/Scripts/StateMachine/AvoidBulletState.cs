using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidBulletState : BaseState
{
    private BulletCollider NearestBullet;
    public AvoidBulletState(EnemyController controller) : base(controller)
    {
    }
    public override StateType GetStateType() => StateType.AvoidBullet;

    public override StateType Tick()
    {
        NearestBullet = controller.NearestBullet;

        if (NearestBullet == null)
            return StateType.Search;

        Vector3 nearestBulletVelocity = NearestBullet.rb.velocity;
        Vector2 direction = Vector2.Perpendicular(new Vector2(nearestBulletVelocity.x, nearestBulletVelocity.y));

        Vector3 moveDirection = new Vector3(direction.x, 0, direction.y);
        if (Vector3.Distance(moveDirection + transform.position, NearestBullet.transform.position) <
            Vector3.Distance(transform.position - moveDirection, NearestBullet.transform.position))
            moveDirection *= -1;

        if(controller)
        Debug.DrawRay(transform.position, moveDirection, Color.blue);
        Debug.DrawRay(NearestBullet.transform.position, moveDirection, Color.blue);

        controller.TargetDestination = transform.position + moveDirection;


        return GetStateType();
    }
}
