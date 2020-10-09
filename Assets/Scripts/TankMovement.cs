using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankMovement : MonoBehaviour
{
    private Rigidbody rb;
    private TankMovementData MovementData;

    public bool debug = false;
    public float MovementForce => MovementData.MovementForce;
    private float MaximumVelcoity => MovementData.MaximumVelocity;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
    }

    public void SetTankMovementData(TankMovementData tankMovementData)
    {
        MovementData = tankMovementData;
    }

    public void SetMovement(Vector3 force)
    {
        rb.AddForce(force);
        if (rb.velocity.magnitude > MaximumVelcoity)
        {
            rb.velocity = rb.velocity.normalized * MaximumVelcoity;
        }
    }
}
