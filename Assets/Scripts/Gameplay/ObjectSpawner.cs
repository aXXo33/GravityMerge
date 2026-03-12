using UnityEngine;
public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private Transform dropPoint, activeObjectsParent;
    [SerializeField] private float dropCooldown=0.8f, clampXMin=-2.5f, clampXMax=2.5f;
    [SerializeField] private int maxSpawnTier=3;
    [SerializeField] private float baseSpecialChance=0.02f, specialGrowth=0.005f;
    private float lastDrop; private GameObject curObj; private MergeObject curMerge;
    private int nextTier; private int extraDrops; private float sessionStart; private bool aiming;
    private void Start(){sessionStart=Time.time;GenNext();SpawnAim();}
    private void OnEnable(){if(GameManager.Instance!=null){GameManager.Instance.OnGameStart+=OnStart;GameManager.Instance.OnGameOver+=OnOver;}}
    private void OnDisable(){if(GameManager.Instance!=null){GameManager.Instance.OnGameStart-=OnStart;GameManager.Instance.OnGameOver-=OnOver;}}
    private void OnStart(){sessionStart=Time.time;lastDrop=0;extraDrops=0;GenNext();SpawnAim();}
    private void OnOver(){if(curObj){Destroy(curObj);curObj=null;}CancelInvoke();}
    private void Update()
    {
        if(GameManager.Instance==null||GameManager.Instance.IsGameOver||GameManager.Instance.CurrentState!=GameManager.GameState.Playing)return;
        if(Input.GetMouseButtonDown(0)&&curObj)aiming=true;
        if(Input.GetMouseButton(0)&&aiming&&curObj){var mp=Camera.main.ScreenToWorldPoint(Input.mousePosition);var cx=Mathf.Clamp(mp.x,clampXMin,clampXMax);var np=new Vector3(cx,dropPoint.position.y,0);dropPoint.position=np;curObj.transform.position=np;}
        if(Input.GetMouseButtonUp(0)&&aiming&&curObj){aiming=false;if(Time.time>=lastDrop+dropCooldown)DropObj();}
    }
    private void GenNext(){float m=(Time.time-sessionStart)/60f;if(Random.value<baseSpecialChance+specialGrowth*m){nextTier=-1;return;}nextTier=Random.Range(0,Mathf.Min(maxSpawnTier,3));if(MergeManager.Instance!=null){var p=MergeManager.Instance.GetTierPrefab(nextTier);if(p){var sr=p.GetComponent<SpriteRenderer>();if(sr)UIManager.Instance?.UpdatePreview(sr.sprite);}}}
    private void SpawnAim(){if(GameManager.Instance.IsGameOver)return;if(nextTier==-1)curObj=SpecialObjectManager.Instance?.SpawnRandomSpecial(dropPoint.position);else{var p=MergeManager.Instance?.GetTierPrefab(nextTier);if(p)curObj=Instantiate(p,dropPoint.position,Quaternion.identity);}if(!curObj)return;if(activeObjectsParent)curObj.transform.SetParent(activeObjectsParent);curMerge=curObj.GetComponent<MergeObject>();if(curMerge)curMerge.PrepareForAiming();}
    private void DropObj(){if(!curObj)return;lastDrop=Time.time;if(curMerge){curMerge.Drop();curMerge.EnableMerging();}else{var rb=curObj.GetComponent<Rigidbody2D>();if(rb)rb.isKinematic=false;var c=curObj.GetComponent<CircleCollider2D>();if(c)c.enabled=true;}curObj=null;curMerge=null;GenNext();Invoke(nameof(SpawnAim),dropCooldown);}
    public void GrantExtraDrops(int c){extraDrops+=c;}
}