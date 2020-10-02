﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : BaseController
{
    private EnemyTankData EnemyTankData => (EnemyTankData)TankData;
    public float MaxVisionDistance => EnemyTankData.maxVisionDistance;
    public float ShootAngle => EnemyTankData.shootAngle;
    public float StrafeDistance => EnemyTankData.strafeDistance;
    public float ChaseDistance => EnemyTankData.chaseDistance;
    public bool _debug => Movement.debug;
    private bool CanMove => EnemyTankData.canMove;
    private float ClosestPlayerOffset => EnemyTankData.closestPlayerOffset;
    public PlayerController TargetedPlayer { get; private set; }
    public BulletCollider NearestBullet { get; private set; }
    public Vector3 TargetDestination { get; set; }


    [SerializeField] private NearestBulletVelocity NearestBulletCollider;
    [SerializeField] private List<StateType> StateTypes;

    private NavMeshAgent NavMeshAgent;
    private StateMachine stateMachine;

    private void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Dictionary<StateType, BaseState> stateMachineDictionary = new Dictionary<StateType, BaseState>();
        foreach (StateType state in StateTypes)
        {
            stateMachineDictionary.Add(state, StateFactory.CreateBaseState(state, this));
        }
        stateMachine = new StateMachine(stateMachineDictionary);
        NavMeshAgent.updateRotation = false;
        NavMeshAgent.acceleration = EnemyTankData.navMeshAcceleration;
        NavMeshAgent.angularSpeed = EnemyTankData.navMeshAngularSpeed;
        NavMeshAgent.speed = EnemyTankData.maximumVelcoity;
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestPlayer(); //For multiplayer will have to check for closets player
        NearestBullet = NearestBulletCollider.Bullet;

        stateMachine.Update();

        if (CanMove)
            Move();
        TargetPlayer(TargetedPlayer.transform);
    }

    /*
    void Update()
    {
        //New Update pattern:
        //Nearest Bullet is updated from Event

        stateMachine.Update();

        if (CanMove) Move();

        if(CheckCurrentShot(currentFacing))
            SetShooting(true)
        else
            RotateHead();
    }
    */

    private void CheckCurrentShot(Vector3 direction)
    {
        //Follow Raycast in direction and calculate reflection angles of bullet
        //If player is hit by raycast
            //shoot and return true
        //else
            //return false
    }
    private void RotateHead()
    {
        //for every y degrees current angle -x degrees to current angle plus x degrees
        //ie (for every 5 degrees from curr_angle - 45 to curr_angle + 45)
            //Follow Raycast direction check if player is hit (CheckCUrrentShot(direction))
            //if true set that as head movement destination
    }

    private void Move()
    {
        NavMeshAgent.SetDestination(TargetDestination);
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

    void TargetPlayer(Transform playerTransform)
    {
        Transform currentTransform = gameObject.transform;

        Vector3 direction = playerTransform.position - currentTransform.position;

        bool canSeePlayer = Physics.Raycast(currentTransform.position, direction, out RaycastHit objectHit, MaxVisionDistance)
            && objectHit.collider.CompareTag("Player");
        if (Movement.debug)
            Debug.DrawRay(currentTransform.position, direction * MaxVisionDistance, Color.green);

        bool setIsShooting = false;
        if (canSeePlayer)
        {
            //if (Movement.debug)
            //    Debug.Log("can see player");
            Debug.DrawRay(currentTransform.position, direction * MaxVisionDistance, Color.green);
            Movement.RotateHead(playerTransform.position);

            float angleDifference = Movement.GetAngleDifference();
            if (Movement.debug)
                Debug.Log("angle: " + angleDifference);
            if (angleDifference <= ShootAngle && angleDifference >= -ShootAngle)
            {
                setIsShooting = true;
            }
        }
        Movement.SetIsShooting(setIsShooting);
    }
}
