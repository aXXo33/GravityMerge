using UnityEngine;
public class MergeManager : MonoBehaviour
{
    public static MergeManager Instance { get; private set; }
    [SerializeField] private TierDatabase tierDatabase;
    [SerializeField] private GameObject[] tierPrefabs;
    [SerializeField] private Transform activeObjectsParent;
    [SerializeField] private float mergePopForce=2.5f, bounceRandom=0.3f;
    private void Awake(){if(Instance==null)Instance=this;else{Destroy(gameObject);return;}if(tierDatabase)tierDatabase.Initialize();}
    public void SpawnNextTier(int next,Vector2 pos)
    {
        if(next>=12)return;
        var o=tierPrefabs!=null&&next<tierPrefabs.Length&&tierPrefabs[next]!=null?Instantiate(tierPrefabs[next],pos,Quaternion.identity):null;
        if(!o)return;if(activeObjectsParent)o.transform.SetParent(activeObjectsParent);
        var mo=o.GetComponent<MergeObject>();if(mo)mo.EnableMerging();
        var rb=o.GetComponent<Rigidbody2D>();if(rb){rb.isKinematic=false;rb.velocity=Vector2.zero;rb.AddForce((Vector2.up+new Vector2(Random.Range(-bounceRandom,bounceRandom),0))*mergePopForce,ForceMode2D.Impulse);}
        var col=o.GetComponent<CircleCollider2D>();if(col)col.enabled=true;
    }
    public GameObject GetTierPrefab(int i)=>(tierPrefabs!=null&&i>=0&&i<tierPrefabs.Length)?tierPrefabs[i]:null;
}