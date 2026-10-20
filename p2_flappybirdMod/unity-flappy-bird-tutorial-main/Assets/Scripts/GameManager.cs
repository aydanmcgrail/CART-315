using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private Spawner spawner;
    [SerializeField] private UtensilSpawner utensilSpawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;

    public int score { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        if (utensilSpawner == null) {
            Debug.LogError("Assign the UtensilSpawner component to the Game Manager.", this);
        }

        Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        player.ShowIdleSprite();
        player.enabled = false;
        spawner.enabled = false;
        if (utensilSpawner != null) {
            utensilSpawner.enabled = false;
        }
    }

    public void Play()
    {
        score = 0;
        scoreText.text = score.ToString();

        playButton.SetActive(false);
        gameOver.SetActive(false);

        Pipes[] pipes = FindObjectsByType<Pipes>(FindObjectsSortMode.None);

        for (int i = 0; i < pipes.Length; i++) {
            Destroy(pipes[i].gameObject);
        }

        Utensil[] utensils = FindObjectsByType<Utensil>(FindObjectsSortMode.None);

        for (int i = 0; i < utensils.Length; i++) {
            Destroy(utensils[i].gameObject);
        }

        Time.timeScale = 1f;
        player.enabled = true;
        spawner.enabled = true;
        if (utensilSpawner != null) {
            utensilSpawner.enabled = true;
        }
    }

    public void GameOver()
    {
        playButton.SetActive(true);
        gameOver.SetActive(true);

        Pause();
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

}
