using UnityEngine;
using System.Collections.Generic;
public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance{get;private set;}
    [SerializeField] private int columns=6,rows=12;
    [SerializeField] private float dangerCheckDelay=1.5f, settleThreshold=0.15f;
    public float BoardFullness{get;private set;}
    private HashSet<MergeObject> dangerObjs=new HashSet<MergeObject>();
    private float dangerTimer; private bool dangerActive;
    private void Awake(){if(Instance==null)Instance=this;else{Destroy(gameObject);return;}}
    private void Update()
    {
        var all=FindObjectsOfType<MergeObject>(); BoardFullness=Mathf.Clamp01(all.Length/(float)(columns*rows));
        if(DangerZoneUI.Instance!=null){if(BoardFullness>=0.95f)DangerZoneUI.Instance.SetDangerLevel(DangerZoneUI.DangerLevel.Critical);else if(BoardFullness>=0.85f)DangerZoneUI.Instance.SetDangerLevel(DangerZoneUI.DangerLevel.Warning);else if(BoardFullness>=0.70f)DangerZoneUI.Instance.SetDangerLevel(DangerZoneUI.DangerLevel.Caution);else DangerZoneUI.Instance.SetDangerLevel(DangerZoneUI.DangerLevel.Safe);}
        if(dangerActive&&dangerObjs.Count>0){dangerTimer+=Time.deltaTime;if(dangerTimer>=dangerCheckDelay){bool s=false;foreach(var o in dangerObjs)if(o&&o.GetComponent<Rigidbody2D>().velocity.magnitude<settleThreshold){s=true;break;}if(s)GameManager.Instance?.TriggerGameOver();else dangerTimer=0;}}
    }
    private void OnTriggerEnter2D(Collider2D c){if(c.TryGetComponent<MergeObject>(out var o))dangerObjs.Add(o);}
    private void OnTriggerStay2D(Collider2D c){if(GameManager.Instance.IsGameOver)return;if(c.TryGetComponent<MergeObject>(out var o)&&o.GetComponent<Rigidbody2D>().velocity.magnitude<settleThreshold&&!dangerActive){dangerActive=true;dangerTimer=0;}}
    private void OnTriggerExit2D(Collider2D c){if(c.TryGetComponent<MergeObject>(out var o)){dangerObjs.Remove(o);if(dangerObjs.Count==0){dangerActive=false;dangerTimer=0;}}}
    public void ClearDangerZone(){var l=new List<MergeObject>(dangerObjs);foreach(var o in l)if(o)o.ReturnSelf();dangerObjs.Clear();dangerActive=false;dangerTimer=0;}
}