using UnityEngine;
using System.Collections.Generic;
public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }
    [System.Serializable] public class Pool { public string tag; public GameObject prefab; public int initialSize = 10; }
    [SerializeField] private List<Pool> pools = new List<Pool>();
    private Dictionary<string, Queue<GameObject>> poolDict = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, Pool> poolLookup = new Dictionary<string, Pool>();
    private Transform poolParent;
    private void Awake()
    {
        if (Instance == null) Instance = this; else { Destroy(gameObject); return; }
        poolParent = new GameObject("_Pool").transform; poolParent.SetParent(transform);
        foreach (var p in pools) { var q = new Queue<GameObject>(); poolLookup[p.tag] = p;
        for (int i = 0; i < p.initialSize; i++) q.Enqueue(MakeNew(p.prefab)); poolDict[p.tag] = q; }
    }
    private GameObject MakeNew(GameObject prefab) { var o = Instantiate(prefab, poolParent); o.SetActive(false); return o; }
    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot)
    {
        if (!poolDict.ContainsKey(tag)) return null;
        var q = poolDict[tag]; var o = q.Count > 0 ? q.Dequeue() : MakeNew(poolLookup[tag].prefab);
        o.transform.position = pos; o.transform.rotation = rot; o.SetActive(true);
        if (o.TryGetComponent<Rigidbody2D>(out var rb)) { rb.velocity = Vector2.zero; rb.angularVelocity = 0f; }
        foreach (var p in o.GetComponents<IPoolable>()) p.OnSpawnFromPool(); return o;
    }
    public void ReturnToPool(GameObject obj)
    {
        if (!obj) return; foreach (var p in obj.GetComponents<IPoolable>()) p.OnReturnToPool();
        obj.SetActive(false); obj.transform.SetParent(poolParent); Destroy(obj);
    }
    public void ReturnToPoolDelayed(GameObject obj, float d) { StartCoroutine(D(obj, d)); }
    private System.Collections.IEnumerator D(GameObject o, float d) { yield return new WaitForSeconds(d); ReturnToPool(o); }
}
public interface IPoolable { void OnSpawnFromPool(); void OnReturnToPool(); }