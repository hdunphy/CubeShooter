using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankMovement : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform headTransform;
    private Rigidbody rb;
    private BaseTankData TankData;

    public bool debug = false;
    public float MovementForce => TankData.movementForce;
    public int NumberOfBullets => TankData.numberOfBullets;
    private float MaximumVelcoity => TankData.maximumVelcoity;
    private float BulletDistanceOffset => TankData.bulletDistanceOffset;
    private float BulletVelocity => TankData.bulletVelocity;
    private float FireRate => TankData.fireRate;
    private float TurnSmoothTime => TankData.turnSmoothTime;
    private int NumberOfBulletBounces => TankData.numberOfBulletBounces;

    private int BulletCount = 0;
    private bool isShooting = false;
    private float nextFire = 0f;
    private float angleDifference = 99999f;
    private float turnSmoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        //rb.inertiaTensor += new Vector3(0, rb.inertiaTensor.y * 100, 0);
    }

    void FixedUpdate()
    {
        if (isShooting && BulletCount < NumberOfBullets && Time.time > nextFire)
        {
            Fire();
            nextFire = Time.time + FireRate;
        }
    }

    public void SetTankData(BaseTankData tankData)
    {
        TankData = tankData;
    }

    void Fire()
    {
        Quaternion headPosition = headTransform.rotation;
        Vector3 velocity = (headPosition * Vector3.forward) * BulletVelocity;
        Vector3 position = transform.position + BulletDistanceOffset * (headPosition * Vector3.forward).normalized;

        GameObject b = BulletObejctPool.Instance.SpawnFromPool(position, headPosition, velocity, BulletVelocity, this, NumberOfBulletBounces);
    }

    public void RotateHead(Vector3 hitPoint)
    {
        Vector3 direction = hitPoint - headTransform.position;
        direction.y = 0;

        if (direction.magnitude >= 0.1f)
        {
            direction.Normalize();
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            RotateHead(targetAngle);

            //angleDifference = Vector3.Angle(direction, headTransform.forward);
            
        }
    }

    public void RotateHead(float targetAngle)
    {
        float angle = Mathf.SmoothDampAngle(headTransform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TurnSmoothTime);
        headTransform.rotation = Quaternion.Euler(0f, angle, 0f);

        float theta = Math.Abs(targetAngle - angle) % 360;
        angleDifference = theta > 180 ? 360 - theta : theta;
        //angleDifference = Mathf.Abs(targetAngle) - Mathf.Abs(angle);
        //angleDifference = Mathf.Abs(targetAngle - angle);
        //angleDifference = Vector3.Angle(direction, headTransform.forward);
    }

    public void RemoveBullet()
    {
        BulletCount--;
        if (BulletCount < 0)
            Debug.LogWarning("Removed too many bullets");
    }

    public void AddBullet()
    {
        BulletCount++;
        if(BulletCount > NumberOfBullets)
            Debug.LogWarning("Added too many bullets");
    }

    public void SetMovement(Vector3 force)
    {
        rb.AddForce(force);
        if (rb.velocity.magnitude > MaximumVelcoity)
        {
            rb.velocity = rb.velocity.normalized * MaximumVelcoity;
        }
    }

    public void SetIsShooting(bool isShooting)
    {
        this.isShooting = isShooting;
    }

    public float GetAngleDifference() { return angleDifference; }

    public int GetNumberOfBulletBounces() { return NumberOfBulletBounces; }

    public Vector3 GetHeadDirection()
    {
        return headTransform.forward;
    }

    public float GetHeadAngle()
    {
        return headTransform.eulerAngles.y;
    }
}
