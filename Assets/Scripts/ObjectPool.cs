using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ObjectPoolData
{
    public GameObject gameObject;
    public int count;

    public ObjectPoolData(GameObject gameObject, int count)
    {
        this.gameObject = gameObject;
        this.count = count;
    }
}

public class ObjectPool : IEnumerable {

    Queue<GameObject> pool = new Queue<GameObject>();

    public ObjectPool (GameObject gameObject, int size)
    {
        for(int i = 0; i < size; i++)
        {
            var pooledObject = GameObject.Instantiate(gameObject);
            pooledObject.SetActive(false);
            pool.Enqueue(pooledObject);
        }
    }

    public ObjectPool (GameObject gameObject, System.Action<GameObject> action, int size)
    {
        for(int i = 0; i < size; i++)
        {
            var pooledObject = GameObject.Instantiate(gameObject);
            action(pooledObject);
            pooledObject.SetActive(false);
            pool.Enqueue(pooledObject);
        }
    }

    public ObjectPool (params ObjectPoolData[] objectData)
    {
        foreach(ObjectPoolData data in objectData)
        {
            for(int i = 0; i < data.count; i++)
            {
                var pooledObject = GameObject.Instantiate(data.gameObject);
                pooledObject.SetActive(false);
                pool.Enqueue(pooledObject);
            }
        }
    }

    public void Shuffle(System.Random random)
    {
        GameObject[] array = pool.ToArray();
        int n = array.Length;
        while(n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            GameObject temp = array[k];
            array[k] = array[n];
            array[n] = temp;
        }

        pool = new Queue<GameObject>(array);
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        var spawnedObject = pool.Dequeue();
        spawnedObject.SetActive(true);
        spawnedObject.transform.position = position;
        spawnedObject.transform.rotation = rotation;

        pool.Enqueue(spawnedObject);

        return spawnedObject;
    }

    public GameObject Spawn(Vector3 position, Vector3 lookAtTarget)
    {
        var spawnedObject = pool.Dequeue();
        spawnedObject.SetActive(true);
        spawnedObject.transform.position = position;
        spawnedObject.transform.LookAt(lookAtTarget);

        pool.Enqueue(spawnedObject);

        return spawnedObject;
    }

    public IEnumerator GetEnumerator()
    {
        return pool.GetEnumerator();
    }
}
