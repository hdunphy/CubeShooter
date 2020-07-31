using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    [SerializeField] protected BaseTankData TankData;
    [SerializeField] protected TankMovement Movement;

    private void Awake()
    {
        Movement.SetTankData(TankData);
    }
}
