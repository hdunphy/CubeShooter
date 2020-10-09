using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMovementData", menuName = "ScriptableObjects/Tanks/EnemyMovementData")]
public class EnemyMovementData : ScriptableObject
{
    [SerializeField] private float navMeshAngularSpeed;
    [SerializeField] private float navMeshAcceleration;
    [SerializeField] private float navMeshVelocity;
    [SerializeField] private float maxVisionDistance;
    [SerializeField] private float closestPlayerOffset;
    [SerializeField] private float strafeDistance;
    [SerializeField] private float chaseDistance;

    public float NavMeshAngularSpeed { get => navMeshAngularSpeed; }
    public float NavMeshAcceleration { get => navMeshAcceleration; }
    public float NavMeshVelocity { get => navMeshVelocity; }
    public float MaxVisionDistance { get => maxVisionDistance; }
    public float ClosestPlayerOffset { get => closestPlayerOffset; }
    public float StrafeDistance { get => strafeDistance; }
    public float ChaseDistance { get => chaseDistance; }

}
