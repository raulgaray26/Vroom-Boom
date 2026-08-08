using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// GameManager definitivo (Integrante 2). Mantiene la misma API pública que el
/// temporal (Instance, AddScore, AddFuel, LoseFuel) para que SpawnManager,
/// CollectibleController y ObstacleController sigan funcionando sin cambios.
/// Agrega: estados de juego, PlayerPrefs para el mejor puntaje, y una dificultad
/// propia (el combustible se consume solo con el tiempo, cada vez más rápido).
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Inicio, Jugando, GameOver }

    [Header("Estado actual (solo lectura durante Play)")]
    public GameState estadoActual = GameState.Inicio;
    public int score = 0;
    public float fuel = 100f;
    public float maxFuel = 100f;
    public bool isGameOver = false;

    [Header("Dificultad propia: consumo pasivo de combustible")]
    [Tooltip("Combustible que se pierde por segundo solo por avanzar, al inicio de la partida.")]
    public float consumoInicial = 2f;
    [Tooltip("Consumo máximo por segundo (tope para que siga siendo jugable).")]
    public float consumoMaximo = 8f;
    [Tooltip("Cuánto aumenta el consumo por segundo transcurrido de partida.")]
    public float aumentoConsumoPorSegundo = 0.03f;

    [Header("Mejor puntaje (PlayerPrefs)")]
    public int highScore = 0;
    private const string HIGH_SCORE_KEY = "VroomBoom_HighScore";

    private float tiempoTranscurrido = 0f;
    private float consumoActual;

    // Eventos estáticos: UIManager y AudioManager se suscriben a esto sin que
    // GameManager necesite conocerlos (bajo acoplamiento, no rompe nada ajeno).
    public static event Action<int> OnScoreChanged;
    public static event Action<float, float> OnFuelChanged; // (fuelActual, fuelMaxima)
    public static event Action OnCollect;      // recolectó moneda o combustible
    public static event Action OnObstacleHit;  // chocó con un obstáculo
    public static event Action OnGameStart;
    public static event Action<int, int, bool> OnGameOverEvent; // (score, highScore, esNuevoRecord)
    public static event Action<GameState> OnStateChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

    void Start()
    {
        // Arranca en pantalla de inicio con el tiempo congelado. No tocamos
        // SpawnManager ni PlayerController: al pausar timeScale, su Update/
        // FixedUpdate reciben deltaTime = 0, así que no se mueven ni spawnean
        // hasta que el jugador presiona "Jugar".
        CambiarEstado(GameState.Inicio);
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (estadoActual != GameState.Jugando) return;

        tiempoTranscurrido += Time.deltaTime;
        consumoActual = Mathf.Min(consumoMaximo, consumoInicial + tiempoTranscurrido * aumentoConsumoPorSegundo);

        AplicarPerdidaFuel(consumoActual * Time.deltaTime);
    }

    /// <summary>Llamado por el botón "Jugar" del UIManager.</summary>
    public void StartGame()
    {
        score = 0;
        fuel = maxFuel;
        isGameOver = false;
        tiempoTranscurrido = 0f;
        consumoActual = consumoInicial;

        Time.timeScale = 1f;
        CambiarEstado(GameState.Jugando);

        OnScoreChanged?.Invoke(score);
        OnFuelChanged?.Invoke(fuel, maxFuel);
        OnGameStart?.Invoke();
    }

    /// <summary>Llamado por el botón "Reiniciar" del UIManager.</summary>
    public void ReiniciarJuego()
    {
        Time.timeScale = 1f; // por si quedó en 0 tras un Game Over
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        OnScoreChanged?.Invoke(score);
        OnCollect?.Invoke();
    }

    public void AddFuel(float amount)
    {
        if (isGameOver) return;
        fuel = Mathf.Min(maxFuel, fuel + amount);
        OnFuelChanged?.Invoke(fuel, maxFuel);
        OnCollect?.Invoke();
    }

    /// <summary>Llamado por ObstacleController al chocar.</summary>
    public void LoseFuel(float amount)
    {
        if (isGameOver) return;
        OnObstacleHit?.Invoke();
        AplicarPerdidaFuel(amount);
    }

    private void AplicarPerdidaFuel(float amount)
    {
        if (isGameOver) return;
        fuel = Mathf.Max(0f, fuel - amount);
        OnFuelChanged?.Invoke(fuel, maxFuel);

        if (fuel <= 0f) TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        if (isGameOver) return; // evita disparar dos veces
        isGameOver = true;

        bool esNuevoRecord = score > highScore;
        if (esNuevoRecord)
        {
            highScore = score;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }

        CambiarEstado(GameState.GameOver);
        Time.timeScale = 0f;

        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.DetenerSpawn();
        }

        OnGameOverEvent?.Invoke(score, highScore, esNuevoRecord);
    }

    private void CambiarEstado(GameState nuevoEstado)
    {
        estadoActual = nuevoEstado;
        OnStateChanged?.Invoke(estadoActual);
    }
}