using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "ScriptableObjects/Tanks/TankMovementData")]
public class TankMovementData : ScriptableObject
{
    [SerializeField] private float movementForce;
    [SerializeField] private float maximumVelcoity;

    public float MovementForce { get { return movementForce; } }
    public float MaximumVelocity { get { return maximumVelcoity; } }
}
