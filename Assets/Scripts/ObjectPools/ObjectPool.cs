using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : Component
{
    [SerializeField]
    private T mPrefab;
    public Transform Transform;
    public Queue<T> ObjectPools = new Queue<T>();
    public static ObjectPool<T> Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public T Get()
    {
        if (ObjectPools.Count == 0)
        {
            AddObejct();
        }
        return ObjectPools.Dequeue();
    }

    private void AddObejct()
    {
        var newPrefab = Instantiate(mPrefab, Transform);
        newPrefab.gameObject.SetActive(false);
        ObjectPools.Enqueue(newPrefab);
    }

    public void ReturnObject(T prefab)
    {
        prefab.gameObject.SetActive(false);
        ObjectPools.Enqueue(prefab);
    }
}