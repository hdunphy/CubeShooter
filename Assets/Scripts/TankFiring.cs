using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFiring : MonoBehaviour
{
    private TankFiringData FiringData;

    public int NumberOfBullets => FiringData.NumberOfBullets;
    public float BulletDistanceOffset => FiringData.BulletDistanceOffset;
    public float BulletVelocity => FiringData.BulletVelocity;
    public float FireRate => FiringData.FireRate;
    public int NumberOfBulletBounces => FiringData.NumberOfBulletBounces;
    public float TurnSmoothTime => FiringData.TurnSmoothTime;

    private float turnSmoothVelocity;

    private bool isShooting = false;
    private float nextFire = 0f;
    private int BulletCount = 0;

    // Update is called once per frame
    void Update()
    {
        if (isShooting && BulletCount < NumberOfBullets && Time.time > nextFire)
        {
            Fire();
            nextFire = Time.time + FireRate;
        }
    }

    void Fire()
    {
        Quaternion headPosition = transform.rotation;
        Vector3 velocity = (headPosition * Vector3.forward) * BulletVelocity;
        Vector3 position = transform.position + BulletDistanceOffset * (headPosition * Vector3.forward).normalized;

        GameObject b = BulletObejctPool.Instance.SpawnFromPool(position, headPosition, velocity, BulletVelocity, this, NumberOfBulletBounces);
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
        if (BulletCount > NumberOfBullets)
            Debug.LogWarning("Added too many bullets");
    }

    public void RotateHead(Vector3 hitPoint)
    {
        Vector3 direction = hitPoint - transform.position;
        direction.y = 0;

        if (direction.magnitude >= 0.1f)
        {
            direction.Normalize();
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            RotateHead(targetAngle);

        }
    }

    public void RotateHead(float targetAngle)
    {
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TurnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //float theta = Math.Abs(targetAngle - angle) % 360;
        //angleDifference = theta > 180 ? 360 - theta : theta;
    }

    public void SetIsShooting(bool isShooting)
    {
        this.isShooting = isShooting;
    }

    public void SetTankFiringData(TankFiringData tankFiringData)
    {
        FiringData = tankFiringData;
    }

    public Vector3 GetHeadDirection()
    {
        return transform.forward;
    }
}
