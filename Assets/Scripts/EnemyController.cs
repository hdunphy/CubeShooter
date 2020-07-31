using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : BaseController
{


    [SerializeField] private NavMeshAgent NavMeshAgent;
    [SerializeField] private NearestBulletVelocity NearestBulletCollider;
    private EnemyTankData EnemyTankData => (EnemyTankData)TankData;

    public float MaxVisionDistance => EnemyTankData.maxVisionDistance;
    public float ShootAngle => EnemyTankData.shootAngle;
    public bool CanMove => EnemyTankData.canMove;
    private float ClosestPlayerOffset => EnemyTankData.closestPlayerOffset;
    
    private PlayerController player;
    private Transform NearestBullet;
    private AIMode currentMode = AIMode.Search;

    enum AIMode { Search, Avoid };

    private void Start()
    {
        NavMeshAgent.updateRotation = false;
        NavMeshAgent.acceleration = EnemyTankData.navMeshAcceleration;
        NavMeshAgent.angularSpeed = EnemyTankData.navMeshAngularSpeed;
        NavMeshAgent.speed = EnemyTankData.maximumVelcoity;
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestPlayer(); //For multiplayer will have to check for closets player
        NearestBullet = NearestBulletCollider.Bullet; //TODO Refactor
        currentMode = NearestBullet == null ? AIMode.Search : AIMode.Avoid;

        //TODO fix this
        Transform playerTransform = player.transform;
        if (CanMove)
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

    private void FindClosestPlayer()
    {
        foreach (PlayerController pc in FindObjectsOfType<PlayerController>())
        {
            if (player == null)
                player = pc;
            else
            {
                float nextPlayerDistance = Mathf.Abs(Vector3.Distance(transform.position, pc.transform.position));
                float currentPlayerDistance = Mathf.Abs(Vector3.Distance(transform.position, player.transform.position));
                if (nextPlayerDistance + ClosestPlayerOffset < currentPlayerDistance)
                    player = pc;
            }
        }
    }

    void FindPlayer(Transform playerTransform)
    {
        Transform currentTransform = gameObject.transform;

        Vector3 direction = playerTransform.position - currentTransform.position;

        bool canSeePlayer = Physics.Raycast(currentTransform.position, direction, out RaycastHit objectHit, MaxVisionDistance)
            && objectHit.collider.CompareTag("Player");

        if (canSeePlayer)
        {
            Debug.DrawRay(currentTransform.position, direction * MaxVisionDistance, Color.green);
            Movement.RotateHead(playerTransform.position);

            float angleDifference = Movement.GetAngleDifference();
            if (angleDifference <= ShootAngle && angleDifference >= -ShootAngle)
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
