using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))][RequireComponent(typeof(CircleCollider2D))]
public class MergeObject : MonoBehaviour, IPoolable
{
    public int tierIndex; public int baseScore; public GameObject mergeVFXPrefab;
    public AudioClip dropSound, mergeSound;
    public SpriteRenderer spriteRenderer;
    public bool canMerge = false;
    private bool isMerging = false, hasLanded = false;
    private Rigidbody2D rb; private CircleCollider2D col; private AudioSource audio;
    private void Awake() { rb=GetComponent<Rigidbody2D>(); col=GetComponent<CircleCollider2D>(); audio=gameObject.AddComponent<AudioSource>(); audio.playOnAwake=false; if(!spriteRenderer)spriteRenderer=GetComponent<SpriteRenderer>(); }
    public void EnableMerging() { canMerge=true; isMerging=false; }
    public void PrepareForAiming() { canMerge=false;isMerging=false;hasLanded=false;rb.isKinematic=true;rb.velocity=Vector2.zero;rb.angularVelocity=0;col.enabled=false; }
    public void Drop() { rb.isKinematic=false;col.enabled=true;rb.AddTorque(Random.Range(-0.5f,0.5f),ForceMode2D.Impulse); }
    private void OnCollisionEnter2D(Collision2D c)
    {
        if(!hasLanded&&c.relativeVelocity.magnitude>1){hasLanded=true;if(dropSound)audio.PlayOneShot(dropSound,Mathf.Clamp01(c.relativeVelocity.magnitude/10f));if(tierIndex>=6)CameraShake.Instance?.Shake(0.05f*tierIndex,0.15f);}
        if(!canMerge||GameManager.Instance.IsGameOver||isMerging) return;
        if(c.gameObject.TryGetComponent<MergeObject>(out var o))
        {
            if(o.tierIndex==tierIndex&&!o.isMerging&&o.canMerge)
            {
                if(tierIndex==11){HandleUniverse(o);return;}
                bool spawn=(transform.position.y<o.transform.position.y)||(Mathf.Approximately(transform.position.y,o.transform.position.y)&&GetInstanceID()<o.GetInstanceID());
                if(spawn){isMerging=true;o.isMerging=true;Merge(o);}
            }
        }
    }
    private void Merge(MergeObject o)
    {
        Vector2 pos=(transform.position+o.transform.position)/2f;
        ComboSystem.Instance?.RegisterMerge();
        int mult=ComboSystem.Instance!=null?ComboSystem.Instance.CurrentMultiplier:1;
        ScoreManager.Instance?.AddScore(baseScore*mult);
        if(mergeVFXPrefab){var v=Instantiate(mergeVFXPrefab,pos,Quaternion.identity);Destroy(v,2f);}
        if(mergeSound)AudioSource.PlayClipAtPoint(mergeSound,pos,0.8f);
        MergeManager.Instance?.SpawnNextTier(tierIndex+1,pos);
        UIManager.Instance?.ShowFloatingScore(pos,baseScore*mult);
        ReturnSelf();o.ReturnSelf();
    }
    private void HandleUniverse(MergeObject o)
    {
        isMerging=true;o.isMerging=true;
        ScoreManager.Instance?.AddScore(TierDatabase.ScoreValues[11]*10);
        GameManager.Instance?.TriggerBoardClear();
        foreach(var x in FindObjectsOfType<MergeObject>())if(x!=this&&x!=o)x.ReturnSelf();
        ReturnSelf();o.ReturnSelf();
    }
    public void ReturnSelf(){canMerge=false;isMerging=false;hasLanded=false;if(ObjectPoolManager.Instance!=null)ObjectPoolManager.Instance.ReturnToPool(gameObject);else Destroy(gameObject);}
    public void OnSpawnFromPool(){canMerge=false;isMerging=false;hasLanded=false;}
    public void OnReturnToPool(){canMerge=false;isMerging=false;hasLanded=false;rb.velocity=Vector2.zero;rb.angularVelocity=0;rb.isKinematic=true;}
}