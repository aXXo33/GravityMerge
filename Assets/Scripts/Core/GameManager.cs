using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { Menu, Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Menu;
    public bool IsGameOver => CurrentState == GameState.GameOver;
    public Action OnGameStart, OnGameOver, OnGamePaused, OnGameResumed, OnBoardCleared;
    [SerializeField] private int extraDropsOnRevive = 3;
    private int reviveCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    private void Start() { StartGame(); }
    public void StartGame()
    {
        CurrentState = GameState.Playing; reviveCount = 0;
        ScoreManager.Instance?.ResetScore(); OnGameStart?.Invoke();
    }
    public void TriggerGameOver()
    {
        if (IsGameOver) return;
        CurrentState = GameState.GameOver; OnGameOver?.Invoke();
    }
    public void PauseGame() { if (CurrentState != GameState.Playing) return; CurrentState = GameState.Paused; Time.timeScale = 0f; OnGamePaused?.Invoke(); }
    public void ResumeGame() { if (CurrentState != GameState.Paused) return; CurrentState = GameState.Playing; Time.timeScale = 1f; OnGameResumed?.Invoke(); }
    public bool TryRevive()
    {
        if (reviveCount >= 1) return false;
        reviveCount++; CurrentState = GameState.Playing;
        BoardManager.Instance?.ClearDangerZone();
        FindObjectOfType<ObjectSpawner>()?.GrantExtraDrops(extraDropsOnRevive);
        return true;
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        foreach (var obj in FindObjectsOfType<MergeObject>()) ObjectPoolManager.Instance?.ReturnToPool(obj.gameObject);
        StartGame();
    }
    public void TriggerBoardClear() { OnBoardCleared?.Invoke(); }
}