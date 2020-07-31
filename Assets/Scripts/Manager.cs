using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] private BulletObejctPool bulletObejctPool;
    // Start is called before the first frame update
    void Start()
    {
        int bullets = 0;
        foreach(TankMovement tank in FindObjectsOfType<TankMovement>())
        {
            bullets += tank.NumberOfBullets;
        }
        bulletObejctPool.CreateInstances(bullets);
    }

}
