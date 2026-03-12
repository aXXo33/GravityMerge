using UnityEngine;
public class SpecialObjectManager : MonoBehaviour
{
    public static SpecialObjectManager Instance{get;private set;}
    [SerializeField] private GameObject gravityBombPrefab,rainbowOrbPrefab,magnetStarPrefab;
    private void Awake(){if(Instance==null)Instance=this;else Destroy(gameObject);}
    public GameObject SpawnRandomSpecial(Vector3 pos){float r=Random.Range(0f,2.5f);var p=r<1?gravityBombPrefab:(r<2?rainbowOrbPrefab:magnetStarPrefab);return p?Instantiate(p,pos,Quaternion.identity):null;}
}