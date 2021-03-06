﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyMovementData enemyMovementData;
    [SerializeField] private TankFiringData tankFiringData;
    [SerializeField] private TankFiring Firing;
    [SerializeField] protected Renderer _renderer;

    public float MaxVisionDistance => tankFiringData.MaxVisionDistance;
    public int SearchAngle => tankFiringData.SearchAngle;
    public float StrafeDistance => enemyMovementData.StrafeDistance;
    public float ChaseDistance => enemyMovementData.ChaseDistance;
    public PlayerController TargetedPlayer { get; private set; }
    public BulletCollider NearestBullet { get; private set; }
    public Vector3 TargetDestination { get; set; }


    [SerializeField] private NearestBulletVelocity NearestBulletCollider;
    [SerializeField] private List<StateType> StateTypes;
    [SerializeField] private bool debug;

    private NavMeshAgent NavMeshAgent;
    private StateMachine stateMachine;
    private bool CanMove;

    private Vector3 ShootDirection = Vector3.zero;
    private const int DegreeOffset = 1;
    private const float ClosestPlayerOffset = 10f;

    private void Awake()
    {
        //Add firing data to Tank Firing
        Firing.SetTankFiringData(tankFiringData);

        //Change color of base tank
        _renderer.material.SetColor("_Color", tankFiringData.TankColor);

        //Attach to nearest bullet event
        NearestBulletCollider.AddDangerousBullet += NearestBulletCollider_AddDangerousBullet;
    }

    private void Start()
    {
        //Set up state machine
        Dictionary<StateType, BaseState> stateMachineDictionary = new Dictionary<StateType, BaseState>();
        foreach (StateType state in StateTypes)
        {
            stateMachineDictionary.Add(state, StateFactory.CreateBaseState(state, this));
        }
        stateMachine = new StateMachine(stateMachineDictionary);

        //Check for NavMeshAgent
        NavMeshAgent = GetComponent<NavMeshAgent>();
        CanMove = NavMeshAgent != null;

        if (CanMove)
        {
            NavMeshAgent.updateRotation = false;
            NavMeshAgent.acceleration = enemyMovementData.NavMeshAcceleration;
            NavMeshAgent.angularSpeed = enemyMovementData.NavMeshAngularSpeed;
            NavMeshAgent.speed = enemyMovementData.NavMeshVelocity;
        }
    }

    private void OnDestroy()
    {
        NearestBulletCollider.AddDangerousBullet -= NearestBulletCollider_AddDangerousBullet;
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestPlayer(); //For multiplayer will have to check for closets player

        stateMachine.Update();

        if (CanMove)
            Move();

        Vector3 headDirection = Firing.GetHeadDirection();
        Vector3 pos = transform.position;

        if (!CheckShot(pos, headDirection * MaxVisionDistance, 0, MaxVisionDistance))
        {
            Firing.SetIsShooting(false);
            Vector3 playerPos = TargetedPlayer.transform.position;
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
                if(FindShot(direction))
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

                    if (debug)
                    {
                        var dir = objectHit.point - from;
                        Debug.DrawRay(from, dir, Color.blue);
                    }
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

                        if (debug && hitPlayer)
                        {
                            Debug.DrawRay(from, dir, Color.yellow);
                        }
                    }
                    break;
            }
        }
        if(hitPlayer)
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

        if (debug)
        {
            var startVector = Quaternion.Euler(0, startAngle, 0) * -direction;
            var endVector = Quaternion.Euler(0, endAngle, 0) * -direction;
            Debug.DrawRay(from, startVector, Color.black);
            Debug.DrawRay(from, endVector, Color.black);
        }

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

    private void Move()
    {
        NavMeshAgent.SetDestination(TargetDestination);
    }

    //Not great because player could be in sight of tank
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

    private void NearestBulletCollider_AddDangerousBullet(BulletCollider bullet)
    {
        NearestBullet = bullet;
    }
}
