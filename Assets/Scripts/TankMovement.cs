using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform headTransform;
    private BaseTankData TankData;

    public bool debug = false;
    public float MovementForce => TankData.movementForce;
    public int NumberOfBullets => TankData.numberOfBullets;
    private float MaximumVelcoity => TankData.maximumVelcoity;
    private float BulletDistanceOffset => TankData.bulletDistanceOffset;
    private float BulletVelocity => TankData.bulletVelocity;
    private float FireRate => TankData.fireRate;
    private float TurnSmoothTime => TankData.turnSmoothTime;

    private int BulletCount = 0;
    private bool isShooting = false;
    private float nextFire = 0f;
    private float angleDifference = 99999f;
    private float turnSmoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Update is called once per frame
    void Update()
    {
        //List<int> indecies = new List<int>();
        //for (int i = 0; i < Bullets.Count(); i++)
        //    if (!Bullets[i].activeInHierarchy)
        //        indecies.Add(i);

        //foreach (int j in indecies)
        //    Bullets.RemoveAt(j);
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

        GameObject b = BulletObejctPool.Instance.SpawnFromPool(position, headPosition, velocity, BulletVelocity, this);
    }

    public void RotateHead(Vector3 hitPoint)
    {
        Vector3 direction = hitPoint - headTransform.position;
        direction.y = 0;

        if (direction.magnitude >= 0.1f)
        {
            direction.Normalize();
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(headTransform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TurnSmoothTime);
            headTransform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            angleDifference = Vector3.Angle(direction, headTransform.forward);
        }
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
}
