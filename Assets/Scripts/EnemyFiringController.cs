using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFiringController : MonoBehaviour
{

    [SerializeField] private TankFiringData FiringData;
    [SerializeField] private TankFiring Firing;
    private Vector3 playerPos;
    private float MaxVisionDistance => FiringData.MaxVisionDistance;
    private int SearchAngle => FiringData.SearchAngle;
    private Vector3 ShootDirection = Vector3.zero;

    private const int DegreeOffset = 1;

    private void Awake()
    {
        Firing.SetTankFiringData(FiringData);
    }

    private void Update()
    {

        Vector3 headDirection = Firing.GetHeadDirection();
        Vector3 pos = transform.position;

        if (!CheckShot(pos, headDirection * MaxVisionDistance, 0, MaxVisionDistance))
        {
            Firing.SetIsShooting(false);
            Vector3 direction = playerPos - pos;

            //if can see player with direct line
            bool canSeePlayer = Physics.Raycast(pos, direction, out RaycastHit objectHit, MaxVisionDistance)
                && objectHit.collider.CompareTag("Player");

            if (canSeePlayer)
            { //move head towards player
                Firing.RotateHead(playerPos);
            }
            else
            {
                if (FindShot(direction))
                    Firing.RotateHead(ShootDirection);
            }
        }
        else
        {
            Firing.SetIsShooting(true);
        }
    }

    private bool CheckShot(Vector3 from, Vector3 to, int currentNumberOfBounces, float distance)
    {
        bool hitPlayer = false;

        //Follow Raycast in direction and calculate reflection angles of bullet
        bool isHit = Physics.Raycast(from, to, out RaycastHit objectHit, distance);

        if (isHit)
        {
            string tag = objectHit.collider.tag;
            switch (tag)
            {
                case "Player":
                    hitPlayer = true;
                    break;

                case "Wall":
                    if (currentNumberOfBounces < Firing.NumberOfBulletBounces)
                    { //If the bullet can bounce check path of the bullet
                        float remDistance = distance - Vector3.Distance(from, objectHit.point);
                        Vector3 newFrom = objectHit.point;
                        Vector3 dir = objectHit.point - from;
                        Vector3 newTo = Vector3.Reflect(dir, objectHit.normal);

                        //Use recurision to follow path of the bullet
                        hitPlayer = CheckShot(newFrom, newTo, ++currentNumberOfBounces, remDistance);

                    }
                    break;
            }
        }
        if (hitPlayer)
        {
            ShootDirection = objectHit.point;
        }
        return hitPlayer;
    }

    private bool FindShot(Vector3 direction)
    {
        Vector3 from, to;
        bool hitPlayer = false;
        int angle;


        from = transform.position;
        int headAngle = Mathf.RoundToInt(Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg);
        int startAngle = headAngle - SearchAngle;
        int endAngle = headAngle + SearchAngle;

        //for every y degrees current angle -x degrees to current angle plus x degrees
        //ie (for every 5 degrees from curr_angle - 45 to curr_angle + 45)
        for (angle = startAngle; angle <= endAngle; angle += DegreeOffset)
        {
            to = Quaternion.Euler(0, angle, 0) * -direction;

            //Follow Raycast direction check if player is hit (CheckCUrrentShot(direction))
            if (CheckShot(from, to, 0, MaxVisionDistance))
            {
                hitPlayer = true;
                break;
            }
        }
        return hitPlayer;
    }
    private void FindClosestPlayer()
    {
        foreach (PlayerController pc in FindObjectsOfType<PlayerController>())
        {
            if (TargetedPlayer == null)
                TargetedPlayer = pc;
            else if (TargetedPlayer != pc)
            {
                float nextPlayerDistance = Mathf.Abs(Vector3.Distance(transform.position, pc.transform.position));
                float currentPlayerDistance = Mathf.Abs(Vector3.Distance(transform.position, TargetedPlayer.transform.position));
                if (nextPlayerDistance + ClosestPlayerOffset < currentPlayerDistance)
                    TargetedPlayer = pc;
            }
        }
    }
}
