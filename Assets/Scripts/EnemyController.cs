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

    private const int DegreeOffset = 5;

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
        //TargetPlayer(TargetedPlayer.transform);

        Vector3 headDirection = Movement.GetHeadDirection();
        //Debug.DrawRay(transform.position, headDirection * MaxVisionDistance, Color.black);
        //bool isHit = Physics.Raycast(transform.position, headDirection, out RaycastHit objectHit, MaxVisionDistance);//, LayerMask.NameToLayer("Default"));
        ////Vector3 newFrom = objectHit.point;
        //if (isHit)
        //{
        //    var normal = objectHit.normal;
        //    //Vector3 direction = transform.position - newFrom;
        //    Vector3 newTo = Vector3.Reflect(headDirection, normal);
        //    Debug.DrawRay(objectHit.point, newTo, Color.blue);
        //}

        //var hit = CheckShot(transform.position, headDirection * MaxVisionDistance, 0);
        if (!CheckShot(transform.position, headDirection * MaxVisionDistance, 0, MaxVisionDistance))
        {
            //Movement.SetIsShooting(false);
            FindShot();
        }
        else
        {
            if (Movement.debug)
                Debug.Log("target");
            //Movement.SetIsShooting(true);
        }
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


    private bool CheckShot(Vector3 from, Vector3 to, int currentNumberOfBounces, float distance)
    {
        bool hitPlayer = false;
        //Follow Raycast in direction and calculate reflection angles of bullet
        bool isHit = Physics.Raycast(from, to, out RaycastHit objectHit, distance);//, LayerMask.NameToLayer("Default"));
        //if (Movement.debug)
        //    Debug.DrawLine(from, to, Color.black);
        if (isHit)
        {
            string tag = objectHit.collider.tag;
            switch (tag)
            {
                case "Player":
                    hitPlayer = true;

                    if (Movement.debug)
                        Debug.DrawRay(from, objectHit.point, Color.yellow);
                    break;
                case "Wall":
                    if (currentNumberOfBounces < Movement.GetNumberOfBulletBounces())
                    { //If the bullet can bounce check path of the bullet
                        float remDistance = distance - Vector3.Distance(from, to);
                        Vector3 newFrom = objectHit.point;
                        Vector3 newTo = Vector3.Reflect(to, objectHit.normal);

                        //Use recurision to follow path of the bullet
                        hitPlayer = CheckShot(newFrom, newTo, ++currentNumberOfBounces, remDistance);

                        if (Movement.debug && hitPlayer)
                            Debug.DrawRay(from, objectHit.point, Color.yellow);
                        else if (Movement.debug)
                            Debug.DrawRay(from, objectHit.point, Color.black);
                    }
                    break;
            }
        }
        return hitPlayer;
    }
    private bool FindShot()
    {
        Vector3 from, to;
        bool hitPlayer = false;
        int angle;

        Vector3 headDirection = to = Movement.GetHeadDirection();
        from = transform.position;
        int headAngle = Mathf.RoundToInt(Mathf.Atan2(headDirection.x, headDirection.z) * Mathf.Rad2Deg);
        int startAngle = headAngle - SearchAngle;
        int endAngle = headAngle + SearchAngle;

        //for every y degrees current angle -x degrees to current angle plus x degrees
        //ie (for every 5 degrees from curr_angle - 45 to curr_angle + 45)
        for (angle = startAngle; angle <= endAngle; angle += DegreeOffset)
        {
            to = headDirection * Mathf.Cos(angle);
            //from.y = 1;
            //to.y = 1;

            //Follow Raycast direction check if player is hit (CheckCUrrentShot(direction))
            if (CheckShot(from, to, 0, MaxVisionDistance))
            {
                hitPlayer = true;
                Movement.RotateHead(to);
                break;
            }
        }

        //if true set that as head movement destination
        //if(to != Vector3.zero)
        //    Movement.RotateHead(to);
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
            //if (Movement.debug)
            //    Debug.Log("can see player");
            Debug.DrawRay(currentTransform.position, direction * MaxVisionDistance, Color.green);
            Movement.RotateHead(playerTransform.position);

            float angleDifference = Movement.GetAngleDifference();
            if (Movement.debug)
                Debug.Log("angle: " + angleDifference);
            if (angleDifference <= ShootAngle)// && angleDifference >= -ShootAngle)
            {
                setIsShooting = true;
            }
        }
        Movement.SetIsShooting(setIsShooting);
    }
}
