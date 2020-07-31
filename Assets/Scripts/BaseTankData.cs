using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseTank", menuName = "ScriptableObjects/Tanks/BaseTank")]
public class BaseTankData : ScriptableObject
{
    public float movementForce;// 500f;
    public float maximumVelcoity;// 5.5f;
    //public float bulletHeight;// 1.5f;
    public float bulletDistanceOffset;// 2.5f;
    public float bulletVelocity;// 7f;
    public float fireRate;// 1f;
    public float turnSmoothTime;// 0.3f;
    public int numberOfBullets;// 5;
}
