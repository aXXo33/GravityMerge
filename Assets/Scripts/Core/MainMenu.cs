using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playBtn;
    [SerializeField] private TextMeshProUGUI highScoreText;
    private void Start()
    {
        if(highScoreText) highScoreText.text="Best: "+PlayerPrefs.GetInt("HighScore",0).ToString("N0");
        if(playBtn) playBtn.onClick.AddListener(()=>SceneManager.LoadScene("2_CoreGame"));
    }
}