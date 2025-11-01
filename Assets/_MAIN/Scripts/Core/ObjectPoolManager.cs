using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private bool addDontDestroyOnLoad = false;
    [SerializeField] private List<GameObject> prefarbToPrewarm;
    [SerializeField] private int prewarmCount = 10;

    private GameObject emptyHolder;

    private static GameObject gameObjects;

    private static Dictionary<GameObject, ObjectPool<GameObject>> pools;
    private static Dictionary<GameObject, GameObject> clones;

    public enum PoolType
    {
        GameObjects,
    }

    public static PoolType poolType;

    void Awake()
    {
        pools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        clones = new Dictionary<GameObject, GameObject>();

        SetupEmptyGameObjects();
    }

    void Start()
    {
        foreach (var prefab in prefarbToPrewarm)
        {
            if (prefab != null)
                Prewarm(prefab, prewarmCount);
        }
    }


    private void SetupEmptyGameObjects()
    {
        emptyHolder = new GameObject("Object Pools");

        gameObjects = new GameObject("Game Objects");
        gameObjects.transform.SetParent(emptyHolder.transform);

        if (addDontDestroyOnLoad)
            DontDestroyOnLoad(gameObjects.transform.root);
    }

    private void Prewarm(GameObject prefab, int count, PoolType poolType = PoolType.GameObjects)
    {
        if (!pools.ContainsKey(prefab))
            CreatePool(prefab, gameObjects.transform, Quaternion.identity, poolType);

        List<GameObject> prewarmedObjects = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = pools[prefab].Get();
            obj.SetActive(false);
            prewarmedObjects.Add(obj);
        }

        foreach (var obj in prewarmedObjects)
            pools[prefab].Release(obj);
    }

    private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () => CreateObject(prefab, pos, rot, poolType),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject
        );

        pools.Add(prefab, pool);

    }
    private static void CreatePool(GameObject prefab, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () => CreateObject(prefab, parent, rot, poolType),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject
        );

        pools.Add(prefab, pool);

    }

    private static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        prefab.SetActive(false);

        GameObject obj = Instantiate(prefab, pos, rot);

        GameObject parent = SetParentObject(poolType);
        obj.transform.SetParent(parent.transform);

        return obj;

    }
    private static GameObject CreateObject(GameObject prefab, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        prefab.SetActive(false);

        GameObject obj = Instantiate(prefab, parent);

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = rot;
        obj.transform.localScale = Vector3.one;


        return obj;

    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.GameObjects:
                return gameObjects;
            default:
                return null;
        }
    }

    private static void OnGetObject(GameObject obj)
    {
        // empty for now
    }

    private static void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    private static void OnDestroyObject(GameObject obj)
    {
        if (clones.ContainsKey(obj))
        {
            clones.Remove(obj);
        }
    }

    private static T SpawnObject<T>(GameObject objectToSpawn, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects) where T : Object
    {
        if (!pools.ContainsKey(objectToSpawn))
        {
            CreatePool(objectToSpawn, pos, rot, poolType);
        }

        GameObject obj = pools[objectToSpawn].Get();

        if (obj != null)
        {
            if (!clones.ContainsKey(obj))
            {
                clones.Add(obj, objectToSpawn);
            }

            obj.transform.rotation = rot;
            obj.transform.localScale = Vector2.one;
            obj.transform.localPosition = pos;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();

            if (component == null)
            {
                Debug.Log($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}");

                return null;
            }

            return component;
        }

        return null;
    }

    public static T SpawnObject<T>(T typePrefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, pos, rot, poolType);
    }
    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        return SpawnObject<GameObject>(objectToSpawn, pos, rot, poolType);
    }

    private static T SpawnObject<T>(GameObject objectToSpawn, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects) where T : Object
    {
        if (!pools.ContainsKey(objectToSpawn))
        {
            CreatePool(objectToSpawn, parent, rot, poolType);
        }

        GameObject obj = pools[objectToSpawn].Get();

        if (obj != null)
        {
            if (!clones.ContainsKey(obj))
            {
                clones.Add(obj, objectToSpawn);
            }

            obj.transform.SetParent(parent, false);
            obj.transform.rotation = rot;
            obj.transform.localScale = Vector2.one;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();

            if (component == null)
            {
                Debug.Log($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}");

                return null;
            }

            return component;
        }

        return null;
    }

    public static T SpawnObject<T>(T typePrefab, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, parent, rot, poolType);
    }
    public static GameObject SpawnObject(GameObject objectToSpawn, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        return SpawnObject<GameObject>(objectToSpawn, parent, rot, poolType);
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        if (clones.TryGetValue(obj, out GameObject prefab))
        {
            GameObject parent = SetParentObject(poolType);

            if (obj.transform.parent != parent.transform)
                obj.transform.SetParent(parent.transform);

            if (pools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
                pool.Release(obj);
            else
                obj.SetActive(false);
        }
        else
            Debug.LogWarning($"Trying to return an object that is not pooled: {obj.name}");
    }

    public static void ReleaseRecursive(GameObject obj)
    {
        if (obj == null) return;



        // Release all child objects first
        for (int i = obj.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = obj.transform.GetChild(i);

            ReleaseRecursive(child.gameObject);
        }

        // Now release the parent
        ReturnObjectToPool(obj);
    }




}
