using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SplashScreen : MonoBehaviour
{
    [SerializeField] private float duration = 2f;
    private void Start() { StartCoroutine(Load()); }
    private IEnumerator Load() { yield return new WaitForSeconds(duration); SceneManager.LoadSceneAsync("1_MainMenu"); }
}