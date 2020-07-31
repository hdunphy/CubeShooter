using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{

    float verticleMovement, horizontalMovement;

    // Update is called once per frame
    void Update()
    {
        verticleMovement = Input.GetAxis("Vertical");
        horizontalMovement = Input.GetAxis("Horizontal");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
            Movement.RotateHead(hit.point);

        Movement.SetIsShooting(Input.GetMouseButton(0));
    }

    private void FixedUpdate()
    {
        float speedModifier = Movement.MovementForce * Time.fixedDeltaTime;
        Vector3 force = new Vector3(Math.Sign(horizontalMovement) * speedModifier, 0, Math.Sign(verticleMovement) * speedModifier);
        Movement.SetMovement(force);
    }


}
