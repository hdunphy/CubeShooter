using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    [SerializeField] protected TankMovementData tankMovementData;
    [SerializeField] protected TankFiringData tankFiringData;
    [SerializeField] protected TankMovement Movement;
    [SerializeField] protected TankFiring Firing;
    [SerializeField] protected Renderer renderer;

    private void Awake()
    {
        if (tankFiringData == null || tankMovementData == null)
            Debug.LogWarning("Missing data");

        if(Movement != null)
            Movement.SetTankMovementData(tankMovementData);
        if (Firing != null)
            Firing.SetTankFiringData(tankFiringData);

        renderer.material.SetColor("_Color", tankFiringData.TankColor);
    }
}
