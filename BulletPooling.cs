using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletPooling : MonoBehaviour
{
    public static BulletPooling SharedInstance;
    public List<GameObject> pooledBullets;
    public GameObject bulletObject;
    public int amountToPool;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledBullets = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(bulletObject);
            tmp.SetActive(false);
            pooledBullets.Add(tmp);
        }
    }

    public GameObject GetPooledBullets()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledBullets[i].activeInHierarchy)
            {
                return pooledBullets[i];
            }
        }
        return null;
    }
}
