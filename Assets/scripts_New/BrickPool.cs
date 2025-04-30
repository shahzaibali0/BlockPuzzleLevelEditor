using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BrickPool
{
    public SingleBrickUi prefab;
    public Transform parent;
    public int initialSize = 20;

    private Queue<SingleBrickUi> pool = new Queue<SingleBrickUi>();

    public void Initialize()
    {
        for (int i = 0; i < initialSize; i++)
        {
            var brick = Object.Instantiate(prefab, parent);
            brick.gameObject.SetActive(false);
            pool.Enqueue(brick);
        }
    }

    public SingleBrickUi Get()
    {
        if (pool.Count == 0)
        {
            // If pool is empty, create a new one
            var brick = Object.Instantiate(prefab, parent);
            return brick;
        }

        var pooledBrick = pool.Dequeue();
        pooledBrick.gameObject.SetActive(true);
        return pooledBrick;
    }

    public void Return(SingleBrickUi brick)
    {
        brick.gameObject.SetActive(false);
        pool.Enqueue(brick);
    }
}