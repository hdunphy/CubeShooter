using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField] private BulletObejctPool bulletObejctPool;

    private bool sceneOver;
    // Start is called before the first frame update
    void Start()
    {
        int bullets = 0;
        foreach(TankMovement tank in FindObjectsOfType<TankMovement>())
        {
            bullets += tank.NumberOfBullets;
        }
        bulletObejctPool.CreateInstances(bullets);
        sceneOver = false;
    }


    private void Update()
    {
        if(!sceneOver && FindObjectsOfType<PlayerController>().Count() <= 0)
            ChangeScene(SceneManager.GetActiveScene().buildIndex);
        else if (!sceneOver && FindObjectsOfType<EnemyController>().Count() <= 0)
            ChangeScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void ChangeScene(int buildIndex)
    {
        sceneOver = true;
        StartCoroutine(WaitInBetweenScenes(5, buildIndex));
    }

    private IEnumerator WaitInBetweenScenes(int seconds, int buildIndex)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(buildIndex);
    }
}
