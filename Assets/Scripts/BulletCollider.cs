using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BulletCollider : MonoBehaviour
{
    public float bulletVelocity;
    private int NumberOfBounces { get; set; }

    public Rigidbody rb;
    [SerializeField] private VisualEffect smoke;

    private TankFiring owner;
    private BulletObejctPool bulletObejctPool;
    private int currentBounces = 0;
    private bool checkVelocity = false;
    private VisualEffect effect;

    private void Start()
    {
        bulletObejctPool = BulletObejctPool.Instance;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (checkVelocity && rb.velocity == Vector3.zero)
            bulletObejctPool.DestroyToPool(gameObject);

    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.collider.tag;
        switch (tag)
        {
            case "Wall":
                if (currentBounces++ < NumberOfBounces)
                    ChangeVelocity(collision);
                else
                    bulletObejctPool.DestroyToPool(gameObject);
                break;
            case "Player":
            case "Tank":
                collision.collider.gameObject.GetComponent<Explosion>().Explode(gameObject);
                bulletObejctPool.DestroyToPool(gameObject);
                break;
            case "Bullet":
                bulletObejctPool.DestroyToPool(gameObject);
                break;
        }
    }

    private void ChangeVelocity(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        var curDir = rb.transform.forward;
        Vector3 newDir = Vector3.Reflect(curDir, contact.normal);
        rb.rotation = Quaternion.FromToRotation(Vector3.forward, newDir);
        rb.velocity = Vector3.zero;
        rb.velocity = newDir.normalized * bulletVelocity;
    }

    public void OnBulletDespawn()
    {
        if (owner != null)
        {
            owner.RemoveBullet();
            owner = null;
        }
        rb.velocity = Vector3.zero;
        bulletVelocity = 0f;
        currentBounces = 0;

        effect.Stop();
        effect.transform.parent = null;

        checkVelocity = false;
    }

    public void OnBulletSpawn(Vector3 velcoity, float maxVelocity, TankFiring tankFiring, int numberOfBounces)
    {
        NumberOfBounces = numberOfBounces;
        owner = tankFiring;
        owner.AddBullet();

        rb.velocity = velcoity;
        bulletVelocity = maxVelocity;

        effect = Instantiate(smoke, transform);
        effect.Play();

        checkVelocity = true;
    }
}
