using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTank", menuName = "ScriptableObjects/Tanks/EnemyTank")]
public class EnemyTankData : BaseTankData
{
    public float navMeshAngularSpeed;
    public float navMeshAcceleration;
    public float maxVisionDistance;// 25f;
    public float shootAngle;// 30f;
    public int searchAngle;
    public bool canMove;// true;
    public float closestPlayerOffset;
    public float strafeDistance;
    public float chaseDistance;
}
