using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    public float cubeSize = 0.25f;
    public int cubesInRow = 5;
    public float explosionForce = 80f;
    public float explosionRadius = 4f;
    public float explosionUpwards = 0.5f;

    public GameObject explosionPieces;

    float cubesPivotDistance;
    Vector3 cubesPivot;

    private void Start()
    {
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);
    }

    public void Explode(GameObject other)
    {
        gameObject.SetActive(false);

        //3 loops to create 5x5x5 pieces in x,y,z coordinates
        for (int x = 0; x < cubesInRow; x++)
            for (int y = 0; y < cubesInRow; y++)
                for (int z = 0; z < cubesInRow; z++)
                    CreatePiece(x, y, z);

        //Get explosion position
        Vector3 explosionPos = other.transform.position;
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, explosionUpwards);
            }
        }
    }

    void CreatePiece(int x, int y, int z)
    {
        GameObject piece = Instantiate(explosionPieces);

        //Set piece position and scale
        piece.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
        piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        //add rigidbody and set mass
        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().mass = cubeSize;
    }
}
