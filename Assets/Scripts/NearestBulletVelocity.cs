using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearestBulletVelocity : MonoBehaviour
{
    public Transform Bullet;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
            Bullet = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bullet"))
            Bullet = null;
    }
}
