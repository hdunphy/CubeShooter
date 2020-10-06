using System;
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
    public int SearchAngle => EnemyTankData.searchAngle;
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

    private Vector3 ShootDirection = Vector3.zero;
    private const int DegreeOffset = 1;

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

        Vector3 headDirection = Movement.GetHeadDirection();
        Vector3 pos = transform.position;

        if (!CheckShot(pos, headDirection * MaxVisionDistance, 0, MaxVisionDistance))
        {
            Movement.SetIsShooting(false);
            Vector3 playerPos = TargetedPlayer.transform.position;
            Vector3 direction = playerPos - pos;

            //if can see player with direct line
            bool canSeePlayer = Physics.Raycast(pos, direction, out RaycastHit objectHit, MaxVisionDistance)
                && objectHit.collider.CompareTag("Player");

            if (canSeePlayer)
            { //move head towards player
                Movement.RotateHead(playerPos);
            }
            else
            {
                if(FindShot(direction))
                    Movement.RotateHead(ShootDirection);
            }
        }
        else
        {
            Movement.SetIsShooting(true);
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

                    if (Movement.debug)
                    {
                        var dir = objectHit.point - from;
                        Debug.DrawRay(from, dir, Color.blue);
                    }
                    break;
                case "Wall":
                    if (currentNumberOfBounces < Movement.GetNumberOfBulletBounces())
                    { //If the bullet can bounce check path of the bullet
                        float remDistance = distance - Vector3.Distance(from, objectHit.point);
                        Vector3 newFrom = objectHit.point;
                        Vector3 dir = objectHit.point - from;
                        Vector3 newTo = Vector3.Reflect(dir, objectHit.normal);

                        //Use recurision to follow path of the bullet
                        hitPlayer = CheckShot(newFrom, newTo, ++currentNumberOfBounces, remDistance);

                        if (Movement.debug && hitPlayer)
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

        if (Movement.debug)
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
            Debug.DrawRay(currentTransform.position, direction * MaxVisionDistance, Color.green);
            Movement.RotateHead(playerTransform.position);

            float angleDifference = Movement.GetAngleDifference();
            if (Movement.debug)
                Debug.Log("angle: " + angleDifference);
            if (angleDifference <= ShootAngle)
            {
                setIsShooting = true;
            }
        }
        Movement.SetIsShooting(setIsShooting);
    }
}
