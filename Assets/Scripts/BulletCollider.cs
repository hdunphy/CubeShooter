using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollider : MonoBehaviour
{
    public float bulletVelocity;
    public int numberOfBounces = 1;

    public Rigidbody rb;

    private int currentBounces = 0;

    private void Start()
    {
        rb.freezeRotation = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.collider.tag;
        switch (tag)
        {
            case "Wall":
                if (currentBounces++ < numberOfBounces)
                    ChangeVelocity(collision);
                else
                    Destroy(gameObject);
                break;
            case "Player":
            case "Tank":
                Debug.Log("Hit a " + tag);
                collision.collider.gameObject.GetComponent<Explosion>().Explode(gameObject);
                Destroy(gameObject);
                break;
            case "Bullet":
                Destroy(gameObject);
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
        //Debug.Log("change velocity: " + rb.velocity.magnitude);
    }
}
