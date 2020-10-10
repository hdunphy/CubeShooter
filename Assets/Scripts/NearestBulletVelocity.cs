using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class NearestBulletVelocity : MonoBehaviour
{
    private BulletCollider Bullet;

    public event Action<BulletCollider> AddDangerousBullet;
    private void OnAddDangerousBulletTrigger(BulletCollider bullet)
    {
        AddDangerousBullet?.Invoke(bullet);
    }

    private void Update()
    {
        if (Bullet != null)
            Debug.DrawRay(Bullet.transform.position, Bullet.transform.forward, Color.red);
    }

    private void OnTriggerExit(Collider other)
    {
        if(Bullet == other.GetComponent<BulletCollider>())
            Bullet = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bullet"))
            SetBullet(other.GetComponent<BulletCollider>());
    }

    private void SetBullet(BulletCollider other)
    {
        Transform otherTransform = other.transform;
        Vector2 perpendicular2 = Vector2.Perpendicular(otherTransform.forward).normalized;
        Vector3 perpendicular = new Vector3(perpendicular2.x, 0f, perpendicular2.y);
        bool hitMe = Physics.Raycast(otherTransform.position, otherTransform.forward, out RaycastHit objectHit, 100f) 
            && objectHit.transform.position == transform.position;
        bool hitMeLeft = Physics.Raycast(otherTransform.position + perpendicular, otherTransform.forward, out RaycastHit objectHitLeft, 100f) 
            && objectHitLeft.transform.position == transform.position;
        bool hitMeRight = Physics.Raycast(otherTransform.position - perpendicular, otherTransform.forward, out RaycastHit objectHitRight, 100f) 
            && objectHitRight.transform.position == transform.position;

        //Debug
        Debug.DrawRay(otherTransform.position, otherTransform.forward, Color.red);
        Debug.DrawRay(otherTransform.position + perpendicular, otherTransform.forward, Color.red);
        Debug.DrawRay(otherTransform.position - perpendicular, otherTransform.forward, Color.red);
        
        
        if (hitMe || hitMeLeft || hitMeRight)
        {
            //Debug.Log("Incoming bullet");
            var bulletDistance = Bullet == null ? 0f : Mathf.Abs(Vector3.Distance(transform.position, Bullet.transform.position));
            var otherDistance = Mathf.Abs(Vector3.Distance(transform.position, otherTransform.position));
            bool bulletIsCloser = Bullet != null && (otherDistance >= bulletDistance);
            if (!bulletIsCloser)
            {
                Bullet = other;
                OnAddDangerousBulletTrigger(Bullet);
            }
        }
        else if (Bullet == other)
        {
            Bullet = null;
            OnAddDangerousBulletTrigger(Bullet);
        }
    }
}
