using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

/// <summary>
/// GameManager definitivo (Integrante 2). Mantiene la misma API pública que
/// SpawnManager, CollectibleController y ObstacleController ya usan.
/// Agrega: estados de juego (Nivel 2 y Victoria), PlayerPrefs para el mejor
/// puntaje, dificultad propia, y el cambio visual de carretera al subir de nivel.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Inicio, Jugando, NivelCompletado, GameOver, Victoria }

    [Header("Estado actual (solo lectura durante Play)")]
    public GameState estadoActual = GameState.Inicio;
    public int score = 0;
    public float fuel = 100f;
    public float maxFuel = 100f;
    public bool isGameOver = false;
    public int nivelActual = 1;

    [Header("Niveles: puntos necesarios")]
    public int puntosParaNivel2 = 200;
    public int puntosParaVictoria = 500;
    public float duracionPantallaNivel = 2f;

    [Header("Visual del Nivel 2 (opcional)")]
    [Tooltip("El GameObject de la carretera del Nivel 1. Debe estar ACTIVO por defecto en la escena.")]
    public GameObject carreteraNivel1;
    [Tooltip("El GameObject de la carretera del Nivel 2 (repintada con tu sprite nuevo). Debe estar INACTIVO por defecto en la escena.")]
    public GameObject carreteraNivel2;

    [Header("Dificultad propia: consumo pasivo de combustible")]
    public float consumoInicial = 2f;
    public float consumoMaximo = 8f;
    public float aumentoConsumoPorSegundo = 0.03f;

    [Header("Dificultad extra al entrar al Nivel 2")]
    public float extraConsumoInicialNivel2 = 1.5f;
    public float extraConsumoMaximoNivel2 = 2f;
    public float extraVelocidadMaximaNivel2 = 2f;
    public float reduccionIntervaloMinimoNivel2 = 0.15f;
    public float reduccionProbabilidadCollectibleNivel2 = 0.1f;

    [Header("Mejor puntaje (PlayerPrefs)")]
    public int highScore = 0;
    private const string HIGH_SCORE_KEY = "VroomBoom_HighScore";

    private float tiempoTranscurrido = 0f;
    private float consumoActual;

    public static event Action<int> OnScoreChanged;
    public static event Action<float, float> OnFuelChanged;
    public static event Action OnCollect;
    public static event Action OnObstacleHit;
    public static event Action OnGameStart;
    public static event Action<int> OnLevelUp;
    public static event Action<int, int, bool> OnGameOverEvent;
    public static event Action<int, int, bool> OnVictoryEvent;
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
        CambiarEstado(GameState.Inicio);
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (estadoActual != GameState.Jugando) return;

        tiempoTranscurrido += Time.deltaTime;
        consumoActual = Mathf.Min(consumoMaximo, consumoInicial + tiempoTranscurrido * aumentoConsumoPorSegundo);
        AplicarPerdidaFuel(consumoActual * Time.deltaTime);

        if (estadoActual != GameState.Jugando) return;

        if (nivelActual == 1 && score >= puntosParaNivel2)
        {
            StartCoroutine(CompletarNivel1());
        }
        else if (nivelActual == 2 && score >= puntosParaVictoria)
        {
            Victoria();
        }
    }

    public void StartGame()
    {
        score = 0;
        fuel = maxFuel;
        isGameOver = false;
        nivelActual = 1;
        tiempoTranscurrido = 0f;
        consumoActual = consumoInicial;

        if (carreteraNivel1 != null) carreteraNivel1.SetActive(true);
        if (carreteraNivel2 != null) carreteraNivel2.SetActive(false);

        Time.timeScale = 1f;
        CambiarEstado(GameState.Jugando);

        OnScoreChanged?.Invoke(score);
        OnFuelChanged?.Invoke(fuel, maxFuel);
        OnGameStart?.Invoke();
    }

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddScore(int amount)
    {
        if (estadoActual != GameState.Jugando) return;
        score += amount;
        OnScoreChanged?.Invoke(score);
        OnCollect?.Invoke();
    }

    public void AddFuel(float amount)
    {
        if (estadoActual != GameState.Jugando) return;
        fuel = Mathf.Min(maxFuel, fuel + amount);
        OnFuelChanged?.Invoke(fuel, maxFuel);
        OnCollect?.Invoke();
    }

    public void LoseFuel(float amount)
    {
        if (estadoActual != GameState.Jugando) return;
        OnObstacleHit?.Invoke();
        AplicarPerdidaFuel(amount);
    }

    private void AplicarPerdidaFuel(float amount)
    {
        if (estadoActual != GameState.Jugando) return;
        fuel = Mathf.Max(0f, fuel - amount);
        OnFuelChanged?.Invoke(fuel, maxFuel);

        if (fuel <= 0f) TriggerGameOver();
    }

    private IEnumerator CompletarNivel1()
    {
        CambiarEstado(GameState.NivelCompletado);
        Time.timeScale = 0f;

        AumentarDificultadNivel2();
        nivelActual = 2;
        OnLevelUp?.Invoke(nivelActual);

        yield return new WaitForSecondsRealtime(duracionPantallaNivel);

        Time.timeScale = 1f;
        CambiarEstado(GameState.Jugando);
    }

    private void AumentarDificultadNivel2()
    {
        consumoInicial += extraConsumoInicialNivel2;
        consumoMaximo += extraConsumoMaximoNivel2;

        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.scrollSpeedMaximo += extraVelocidadMaximaNivel2;
            SpawnManager.Instance.spawnIntervalMinimo = Mathf.Max(0.25f,
                SpawnManager.Instance.spawnIntervalMinimo - reduccionIntervaloMinimoNivel2);
            SpawnManager.Instance.probabilidadCollectible = Mathf.Max(0.2f,
                SpawnManager.Instance.probabilidadCollectible - reduccionProbabilidadCollectibleNivel2);
        }

        // Cambio visual: mostramos la carretera repintada del Nivel 2.
        if (carreteraNivel1 != null) carreteraNivel1.SetActive(false);
        if (carreteraNivel2 != null) carreteraNivel2.SetActive(true);
    }

    private void TriggerGameOver()
    {
        if (estadoActual == GameState.GameOver) return;
        isGameOver = true;

        bool esNuevoRecord = GuardarHighScoreSiCorresponde();

        CambiarEstado(GameState.GameOver);
        Time.timeScale = 0f;

        if (SpawnManager.Instance != null) SpawnManager.Instance.DetenerSpawn();

        OnGameOverEvent?.Invoke(score, highScore, esNuevoRecord);
    }

    private void Victoria()
    {
        if (estadoActual == GameState.Victoria) return;

        bool esNuevoRecord = GuardarHighScoreSiCorresponde();

        CambiarEstado(GameState.Victoria);
        Time.timeScale = 0f;

        if (SpawnManager.Instance != null) SpawnManager.Instance.DetenerSpawn();

        OnVictoryEvent?.Invoke(score, highScore, esNuevoRecord);
    }

    private bool GuardarHighScoreSiCorresponde()
    {
        bool esNuevoRecord = score > highScore;
        if (esNuevoRecord)
        {
            highScore = score;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }
        return esNuevoRecord;
    }

    private void CambiarEstado(GameState nuevoEstado)
    {
        estadoActual = nuevoEstado;
        OnStateChanged?.Invoke(estadoActual);
    }
}