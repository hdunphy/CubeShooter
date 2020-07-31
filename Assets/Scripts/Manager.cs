using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField] private BulletObejctPool bulletObejctPool;
    // Start is called before the first frame update
    void Start()
    {
        int bullets = 0;
        foreach(TankMovement tank in FindObjectsOfType<TankMovement>())
        {
            bullets += tank.NumberOfBullets;
        }
        bulletObejctPool.CreateInstances(bullets);
    }


    private void Update()
    {
        if(FindObjectsOfType<PlayerController>().Count() <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else if (FindObjectsOfType<EnemyController>().Count() <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
