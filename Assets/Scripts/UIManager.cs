using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla las 3 pantallas de UI (Inicio, HUD, Game Over) escuchando los
/// eventos estáticos de GameManager. No busca a GameManager directamente
/// para actualizarse: solo reacciona a lo que él avisa.
///
/// CÓMO USARLO:
/// 1. Crea un Canvas con 3 GameObjects hijos: PanelInicio, PanelHUD, PanelGameOver.
/// 2. Arrastra cada uno y sus textos/botones a los campos del Inspector.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Panel Inicio")]
    public GameObject panelInicio;
    public TMP_Text textoHighScoreInicio;
    public Button botonJugar;

    [Header("Panel HUD (durante el juego)")]
    public GameObject panelHUD;
    public TMP_Text textoScore;
    public Slider barraFuel; // Min Value = 0, Max Value = 1
    public TMP_Text textoFuel;

    [Header("Panel Game Over")]
    public GameObject panelGameOver;
    public TMP_Text textoScoreFinal;
    public TMP_Text textoHighScoreFinal;
    public GameObject textoNuevoRecord; // se activa/desactiva según corresponda
    public Button botonReiniciar;

    void OnEnable()
    {
        GameManager.OnScoreChanged += ActualizarScore;
        GameManager.OnFuelChanged += ActualizarFuel;
        GameManager.OnStateChanged += ActualizarPantalla;
        GameManager.OnGameOverEvent += MostrarGameOver;
    }

    void OnDisable()
    {
        GameManager.OnScoreChanged -= ActualizarScore;
        GameManager.OnFuelChanged -= ActualizarFuel;
        GameManager.OnStateChanged -= ActualizarPantalla;
        GameManager.OnGameOverEvent -= MostrarGameOver;
    }

    void Start()
    {
        if (textoHighScoreInicio != null && GameManager.Instance != null)
        {
            textoHighScoreInicio.text = "Mejor puntaje: " + GameManager.Instance.highScore;
        }

        botonJugar.onClick.AddListener(() => GameManager.Instance.StartGame());
        botonReiniciar.onClick.AddListener(() => GameManager.Instance.ReiniciarJuego());

        ActualizarPantalla(GameManager.GameState.Inicio);
    }

    private void ActualizarScore(int nuevoScore)
    {
        if (textoScore != null) textoScore.text = "Puntaje: " + nuevoScore;
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
        panelGameOver.SetActive(estado == GameManager.GameState.GameOver);
    }

    private void MostrarGameOver(int scoreFinal, int highScore, bool esNuevoRecord)
    {
        if (textoScoreFinal != null) textoScoreFinal.text = "Puntaje: " + scoreFinal;
        if (textoHighScoreFinal != null) textoHighScoreFinal.text = "Mejor puntaje: " + highScore;
        if (textoNuevoRecord != null) textoNuevoRecord.SetActive(esNuevoRecord);
    }
}