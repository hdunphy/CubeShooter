using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public float movementForce = 500f;
    public float maximumVelcoity = 5.5f;
    public float bulletHeight = 1.5f;
    public float bulletDistanceOffset = 2.5f;
    public float bulletVelocity = 7f;
    public float fireRate = 1f;
    public float turnSmoothTime = 0.3f;
    public int numberOfBullets = 5;
    public bool debug = false;

    public GameObject bullet;
    public Rigidbody rb;

    private Transform headTransform;
    private List<GameObject> Bullets;
    private bool isShooting = false;
    private float nextFire = 0f;
    private float angleDifference = 99999f;
    private float turnSmoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        headTransform = transform.Find("Head");
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        Bullets = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        List<int> indecies = new List<int>();
        for (int i = 0; i < Bullets.Count() - 1; i++)
            if (Bullets[i] == null)
                indecies.Add(i);

        foreach (int j in indecies)
            Bullets.RemoveAt(j);
    }

    void FixedUpdate()
    {
        if (isShooting && Bullets.Count() < numberOfBullets && Time.time > nextFire)
        {
            Fire();
            nextFire = Time.time + fireRate;
        }
    }

    void Fire()
    {
        if (debug)
            Debug.Log("Shoot");
        Quaternion headPosition = headTransform.rotation;
        Vector3 velocity = (headPosition * Vector3.forward) * bulletVelocity;
        Vector3 position = transform.position + bulletDistanceOffset * (headPosition * Vector3.forward).normalized;

        GameObject b = Instantiate(bullet, position, headPosition);
        b.GetComponent<Rigidbody>().velocity = velocity;
        b.GetComponent<BulletCollider>().bulletVelocity = bulletVelocity;
        Bullets.Add(b);
        if (debug)
            Debug.Log(Bullets.Count());
    }

    public void RotateHead(Vector3 hitPoint)
    {
        Vector3 direction = hitPoint - headTransform.position;
        direction.y = 0;

        if (direction.magnitude >= 0.1f)
        {
            direction.Normalize();
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(headTransform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            headTransform.rotation = Quaternion.Euler(0f, angle, 0f);
            //angleDifference = Mathf.Abs(headTransform.rotation.eulerAngles.y - angle);
            
            angleDifference = Vector3.Angle(direction, headTransform.forward);
        }
    }

    public void SetMovement(Vector3 force)
    {
        rb.AddForce(force);
        if (rb.velocity.magnitude > maximumVelcoity)
        {
            //Debug.Log("Capped Velocity");
            rb.velocity = rb.velocity.normalized * maximumVelcoity;
        }
    }

    public void SetIsShooting(bool isShooting)
    {
        this.isShooting = isShooting;
    }

    public float GetAngleDifference() { return angleDifference; }
}
