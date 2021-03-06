﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] protected TankMovementData tankMovementData;
    [SerializeField] protected TankFiringData tankFiringData;
    [SerializeField] protected TankMovement Movement;
    [SerializeField] protected TankFiring Firing;
    [SerializeField] protected Renderer _renderer;

    float verticleMovement, horizontalMovement;
    private void Awake()
    {
        if (tankFiringData == null || tankMovementData == null)
            Debug.LogWarning("Missing data");

        Movement.SetTankMovementData(tankMovementData);
        Firing.SetTankFiringData(tankFiringData);

        _renderer.material.SetColor("_Color", tankFiringData.TankColor);
    }

    // Update is called once per frame
    void Update()
    {
        verticleMovement = Input.GetAxis("Vertical");
        horizontalMovement = Input.GetAxis("Horizontal");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
            Firing.RotateHead(hit.point);

        Firing.SetIsShooting(Input.GetMouseButton(0));
    }

    private void FixedUpdate()
    {
        float speedModifier = Movement.MovementForce * Time.fixedDeltaTime;
        Vector3 force = new Vector3(Math.Sign(horizontalMovement) * speedModifier, 0, Math.Sign(verticleMovement) * speedModifier);
        Movement.SetMovement(force);
    }


}
