using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public TankMovement Movement;
    public float maxVisionDistance = 25f;
    public float shootAngle = 30f;
    public bool canMove = true;

    private GameObject player;
    private Rigidbody playerRB;
    private NavMeshAgent NavMeshAgent;
    private Transform NearestBullet;
    private AIMode currentMode = AIMode.Search;

    enum AIMode { Search, Avoid };

    void Awake()
    {
        player = GameObject.Find("Player"); //For multiplayer will have to check for closets player
        playerRB = player.GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        NearestBullet = transform.Find("Base").Find("BulletDetector").GetComponent<NearestBulletVelocity>().Bullet;
        currentMode = NearestBullet == null ? AIMode.Search : AIMode.Avoid;

        Transform playerTransform = player.transform;
        if (canMove)
            Move(playerTransform);
        FindPlayer(playerTransform);
    }

    private void Move(Transform playerTransform)
    {
        if (currentMode == AIMode.Search)
            NavMeshAgent.SetDestination(playerTransform.position);
        else if (currentMode == AIMode.Avoid)
        {
            Vector2 direction = Vector2.Perpendicular(new Vector2(NearestBullet.position.x, NearestBullet.position.z));
            Movement.SetMovement(new Vector3(direction.x, 0f, direction.y));
        }
    }

    void FindPlayer(Transform playerTransform)
    {
        Transform currentTransform = gameObject.transform;

        Vector3 direction = playerTransform.position - currentTransform.position;
        float timeBulletToPlayer = (direction).magnitude / Movement.bulletVelocity;

        // Project future position
        Vector3 futurePos = playerTransform.position;// + playerRB.velocity * timeBulletToPlayer;
        Vector3 destination = futurePos - currentTransform.position;

        bool canSeePlayer = Physics.Raycast(currentTransform.position, direction * maxVisionDistance, out RaycastHit objectHit)
            && objectHit.collider.CompareTag("Player");

        if (canSeePlayer)
        {
            Debug.DrawRay(currentTransform.position, destination * maxVisionDistance, Color.green);
            Movement.RotateHead(futurePos);

            //bool canHitPlayer = Physics.Raycast(currentTransform.position, destination * maxVisionDistance, out RaycastHit objectHit1)
            //    && objectHit1.collider.CompareTag("Player"); // && canHitPlayer
            if (Movement.GetAngleDifference() <= shootAngle && Movement.GetAngleDifference() >= -shootAngle)
            {
                Movement.SetIsShooting(true);
            }
            else
            {
                Movement.SetIsShooting(false);
            }
        }
        else
        {
            Movement.SetIsShooting(false);
        }
    }
}
