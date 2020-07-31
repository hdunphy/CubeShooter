using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearestBulletVelocity : MonoBehaviour
{
    public Transform Bullet { get; private set; }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Bullet"))
    //        Bullet = other.transform;
    //}

    private void Update()
    {
        if (Bullet != null)
            Debug.DrawRay(Bullet.position, Bullet.forward, Color.red);
    }

    private void OnTriggerExit(Collider other)
    {
        if(Bullet == other.transform)
            Bullet = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bullet"))
            SetBullet(other.transform);
    }

    private void SetBullet(Transform other)
    {
        Vector2 perpendicular2 = Vector2.Perpendicular(other.forward).normalized;
        Vector3 perpendicular = new Vector3(perpendicular2.x, 0f, perpendicular2.y);
        bool hitMe = Physics.Raycast(other.position, other.forward, out RaycastHit objectHit, 100f) 
            && objectHit.transform.position == transform.position;
        bool hitMeLeft = Physics.Raycast(other.position + perpendicular, other.forward, out RaycastHit objectHitLeft, 100f) 
            && objectHitLeft.transform.position == transform.position;
        bool hitMeRight = Physics.Raycast(other.position - perpendicular, other.forward, out RaycastHit objectHitRight, 100f) 
            && objectHitRight.transform.position == transform.position;
        Debug.DrawRay(other.position, other.forward, Color.red);
        Debug.DrawRay(other.position + perpendicular, other.forward, Color.red);
        Debug.DrawRay(other.position - perpendicular, other.forward, Color.red);
        if (hitMe || hitMeLeft || hitMeRight)
        {
            //Debug.Log("Incoming bullet");
            var bulletDistance = Bullet == null ? 0f : Mathf.Abs(Vector3.Distance(transform.position, Bullet.position));
            var otherDistance = Mathf.Abs(Vector3.Distance(transform.position, other.position));
            bool bulletIsCloser = Bullet != null && (otherDistance >= bulletDistance);
            if (!bulletIsCloser)
                Bullet = other;
        }
        else if (Bullet == other)
            Bullet = null;
    }
}
