using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FiringData", menuName = "ScriptableObjects/Tanks/TankFiring")]
public class TankFiringData : ScriptableObject
{
    [SerializeField] private float turnSmoothTime;
    [SerializeField] private float bulletDistanceOffset;
    [SerializeField] private float bulletVelocity;// 7f;
    [SerializeField] private float fireRate;// 1f;
    [SerializeField] private float maxVisionDistance;
    [SerializeField] private int numberOfBullets;// 5;
    [SerializeField] private int numberOfBulletBounces;
    [SerializeField] private Color color;

    [Range(0, 180), SerializeField]
    private int searchAngle;

    public float TurnSmoothTime { get { return turnSmoothTime; } }
    public float BulletDistanceOffset { get => bulletDistanceOffset; }
    public float BulletVelocity { get => bulletVelocity; }
    public float FireRate { get => fireRate; }
    public float MaxVisionDistance { get => maxVisionDistance; }
    public int NumberOfBullets { get => numberOfBullets; }
    public int NumberOfBulletBounces { get => numberOfBulletBounces; }
    public int SearchAngle { get => searchAngle; }
    public Color TankColor { get => color; }
}
