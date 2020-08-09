using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankMovement))]
public class BaseController : MonoBehaviour
{
    [SerializeField] protected BaseTankData TankData;
    protected TankMovement Movement;

    private void Awake()
    {
        Movement = GetComponent<TankMovement>();
        Movement.SetTankData(TankData);
    }
}
