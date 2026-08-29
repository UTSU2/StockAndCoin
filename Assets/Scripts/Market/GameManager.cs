using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private MarketSimulator marketSimulator;
    [SerializeField] private UIController uiController;

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void InitializeGame()
    {
        IsPaused = false;

        Time.timeScale = 1f;

        if (uiController != null)
            uiController.RefreshUI();
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        if (IsPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}