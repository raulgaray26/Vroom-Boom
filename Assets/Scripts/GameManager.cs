using UnityEngine;

/// <summary>
/// ESTE SCRIPT ES TEMPORAL
/// No es responsabilidad del Integrante 1 (es parte del bloque del Integrante 2:
/// GameManager, UIManager, AudioManager), pero SpawnManager, CollectibleController y
/// ObstacleController necesitan un GameManager.Instance con estos métodos públicos
/// para poder compilar y probarse de forma aislada mientras tu compañero no ha
/// escrito su versión definitiva.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Estado (solo para pruebas — el definitivo lo maneja el Integrante 2)")]
    public int score = 0;
    public float fuel = 100f;
    public float maxFuel = 100f;
    public bool isGameOver = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        Debug.Log("Puntaje: " + score);
    }

    public void AddFuel(float amount)
    {
        if (isGameOver) return;
        fuel = Mathf.Min(maxFuel, fuel + amount);
        Debug.Log("Combustible: " + fuel);
    }

    public void LoseFuel(float amount)
    {
        if (isGameOver) return;
        fuel = Mathf.Max(0f, fuel - amount);
        Debug.Log("Combustible: " + fuel);

        if (fuel <= 0f)
        {
            TriggerGameOverTemporal();
        }
    }

    private void TriggerGameOverTemporal()
    {
        isGameOver = true;
        Debug.Log("GAME OVER (placeholder) — puntaje final: " + score);
        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.DetenerSpawn();
        }
        // El Integrante 2 reemplazará esto por: mostrar pantalla de Game Over,
        // guardar el mejor puntaje con PlayerPrefs, detener música, etc.
    }
}
