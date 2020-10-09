using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObejctPool : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    private Queue<GameObject> bulletQueue;

    public static BulletObejctPool Instance;

    public void Awake()
    {
        bulletQueue = new Queue<GameObject>();
        //Singleton
        Instance = this;
    }

    public void CreateInstances(int instances)
    {
        for (int i = 0; i < instances; i++)
        {
            var obj = Instantiate(prefab);
            obj.SetActive(false);
            bulletQueue.Enqueue(obj);
        }
    }

    public GameObject SpawnFromPool(Vector3 position, Quaternion rotation, Vector3 velocity, float maxVelocity, TankFiring tankFiring, int numberOfBounces)
    {
        GameObject bullet = bulletQueue.Dequeue();
        bullet.SetActive(true);
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
        bullet.GetComponent<BulletCollider>().OnBulletSpawn(velocity, maxVelocity, tankFiring, numberOfBounces);
        return bullet;
    }

    public void DestroyToPool(GameObject bullet)
    {
        bullet.GetComponent<BulletCollider>().OnBulletDespawn();
        bullet.SetActive(false);
        bulletQueue.Enqueue(bullet);
    }
}
