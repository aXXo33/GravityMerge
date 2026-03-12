using UnityEngine;
using System.Collections;
public class ShareManager : MonoBehaviour
{
    public static ShareManager Instance { get; private set; }
    [SerializeField] private string shareTemplate = "I scored {0} points in Gravity Merge! #GravityMerge";
    private void Awake() { if(Instance==null)Instance=this;else Destroy(gameObject); }
    public void ShareScore(int score) { GUIUtility.systemCopyBuffer = string.Format(shareTemplate, score.ToString("N0")); }
}