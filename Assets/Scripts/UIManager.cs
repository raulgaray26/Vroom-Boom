using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Controla las 5 pantallas (Inicio, HUD, Next Level, Game Over, Victoria)
/// escuchando los eventos estáticos de GameManager.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Panel Inicio")]
    public GameObject panelInicio;
    public TMP_Text textoHighScoreInicio; // opcional, puede quedar sin asignar
    public Button botonJugar;

    [Header("Panel HUD (durante el juego)")]
    public GameObject panelHUD;
    public TMP_Text textoScore;
    public TMP_Text textoObjetivo;
    public TMP_Text textoHighScoreHUD;
    public Slider barraFuel;
    public TMP_Text textoFuel;

    [Header("Panel Next Level (transición automática)")]
    public GameObject panelNivelCompletado;

    [Header("Panel Game Over")]
    public GameObject panelGameOver;
    public TMP_Text textoScoreFinal;
    public TMP_Text textoHighScoreFinal;
    public GameObject textoNuevoRecord;
    public Button botonReiniciar;

    [Header("Panel Victoria")]
    public GameObject panelVictoria;
    public TMP_Text textoScoreVictoria;
    public TMP_Text textoHighScoreVictoria;
    public GameObject textoNuevoRecordVictoria;
    public Button botonReiniciarVictoria;

    [Header("Animación de Victoria")]
    public float duracionAnimacionVictoria = 0.5f;

    private Coroutine animacionVictoriaCoroutine;

    void OnEnable()
    {
        GameManager.OnScoreChanged += ActualizarScore;
        GameManager.OnFuelChanged += ActualizarFuel;
        GameManager.OnStateChanged += ActualizarPantalla;
        GameManager.OnLevelUp += ManejarNivelUp;
        GameManager.OnGameOverEvent += MostrarGameOver;
        GameManager.OnVictoryEvent += MostrarVictoria;
    }

    void OnDisable()
    {
        GameManager.OnScoreChanged -= ActualizarScore;
        GameManager.OnFuelChanged -= ActualizarFuel;
        GameManager.OnStateChanged -= ActualizarPantalla;
        GameManager.OnLevelUp -= ManejarNivelUp;
        GameManager.OnGameOverEvent -= MostrarGameOver;
        GameManager.OnVictoryEvent -= MostrarVictoria;
    }

    void Start()
    {
        if (textoHighScoreInicio != null && GameManager.Instance != null)
        {
            textoHighScoreInicio.text = "Mejor puntaje: " + GameManager.Instance.highScore;
        }

        botonJugar.onClick.AddListener(() => GameManager.Instance.StartGame());
        botonReiniciar.onClick.AddListener(() => GameManager.Instance.ReiniciarJuego());
        botonReiniciarVictoria.onClick.AddListener(() => GameManager.Instance.ReiniciarJuego());

        ActualizarPantalla(GameManager.GameState.Inicio);
    }

    private void ActualizarScore(int nuevoScore)
    {
        if (textoScore != null) textoScore.text = "Puntaje: " + nuevoScore;
        ActualizarObjetivo(nuevoScore);
        ActualizarHighScoreHUD(nuevoScore);
    }

    private void ActualizarHighScoreHUD(int nuevoScore)
    {
        if (textoHighScoreHUD == null || GameManager.Instance == null) return;

        // Muestra el mayor entre tu puntaje actual y tu récord guardado,
        // así si lo superas a mitad de partida, se actualiza al instante.
        int mejor = Mathf.Max(nuevoScore, GameManager.Instance.highScore);
        textoHighScoreHUD.text = "Mejor puntaje: " + mejor;
    }

    private void ActualizarObjetivo(int nuevoScore)
    {
        if (textoObjetivo == null || GameManager.Instance == null) return;

        int nivel = GameManager.Instance.nivelActual;
        int meta = nivel == 1 ? GameManager.Instance.puntosParaNivel2 : GameManager.Instance.puntosParaVictoria;
        textoObjetivo.text = "Nivel " + nivel + " - Meta: " + nuevoScore + " / " + meta;
    }

    private void ManejarNivelUp(int nuevoNivel)
    {
        if (GameManager.Instance != null) ActualizarObjetivo(GameManager.Instance.score);
    }

    private void ActualizarFuel(float fuelActual, float fuelMaxima)
    {
        if (barraFuel != null) barraFuel.value = fuelMaxima > 0 ? fuelActual / fuelMaxima : 0f;
        if (textoFuel != null) textoFuel.text = Mathf.CeilToInt(fuelActual) + " / " + Mathf.CeilToInt(fuelMaxima);
    }

    private void ActualizarPantalla(GameManager.GameState estado)
    {
        panelInicio.SetActive(estado == GameManager.GameState.Inicio);
        panelHUD.SetActive(estado == GameManager.GameState.Jugando);
        panelNivelCompletado.SetActive(estado == GameManager.GameState.NivelCompletado);
        panelGameOver.SetActive(estado == GameManager.GameState.GameOver);
        panelVictoria.SetActive(estado == GameManager.GameState.Victoria);
    }

    private void MostrarGameOver(int scoreFinal, int highScore, bool esNuevoRecord)
    {
        if (textoScoreFinal != null) textoScoreFinal.text = "Puntaje: " + scoreFinal;
        if (textoHighScoreFinal != null) textoHighScoreFinal.text = "Mejor puntaje: " + highScore;
        if (textoNuevoRecord != null) textoNuevoRecord.SetActive(esNuevoRecord);
    }

    private void MostrarVictoria(int scoreFinal, int highScore, bool esNuevoRecord)
    {
        if (textoScoreVictoria != null) textoScoreVictoria.text = "Puntaje total: " + scoreFinal;
        if (textoHighScoreVictoria != null) textoHighScoreVictoria.text = "Mejor puntaje: " + highScore;
        if (textoNuevoRecordVictoria != null) textoNuevoRecordVictoria.SetActive(esNuevoRecord);

        if (animacionVictoriaCoroutine != null) StopCoroutine(animacionVictoriaCoroutine);
        animacionVictoriaCoroutine = StartCoroutine(AnimarEntradaVictoria());
    }

    private IEnumerator AnimarEntradaVictoria()
    {
        if (panelVictoria == null) yield break;

        Transform t = panelVictoria.transform;
        t.localScale = Vector3.zero;

        float tiempo = 0f;
        while (tiempo < duracionAnimacionVictoria)
        {
            tiempo += Time.unscaledDeltaTime;
            float progreso = Mathf.Clamp01(tiempo / duracionAnimacionVictoria);
            float escala = EaseOutBack(progreso);
            t.localScale = Vector3.one * escala;
            yield return null;
        }

        t.localScale = Vector3.one;
    }

    private float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3) + c1 * Mathf.Pow(x - 1f, 2);
    }
}