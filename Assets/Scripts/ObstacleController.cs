using UnityEngine;

/// <summary>
/// Controla el comportamiento de un obstáculo (cono, barril, vehículo detenido, barril explosivo):
/// - Se mueve hacia abajo simulando el avance del carro.
/// - Se destruye si sale de la pantalla sin chocar (evita fugas de memoria).
/// - Al chocar con el jugador: resta combustible (o termina el juego de inmediato si es explosivo).
/// </summary>
public class ObstacleController : MonoBehaviour
{
    public enum ObstacleType { Normal, Explosivo }

    [Header("Configuración")]
    public ObstacleType type = ObstacleType.Normal;

    [Tooltip("Combustible que resta al chocar (solo aplica si Type = Normal).")]
    public float fuelPenalty = 30f;

    [Header("Límite inferior de pantalla")]
    public float destroyY = -6f;

    [Header("Efecto opcional")]
    [Tooltip("Prefab de explosión/partícula que se instancia al chocar (opcional, puede quedar vacío).")]
    public GameObject explosionEffectPrefab;

    private bool yaColisiono = false;

    void Update()
    {
        float speed = SpawnManager.Instance != null ? SpawnManager.Instance.GetScrollSpeed() : 4f;
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (yaColisiono) return; // evita procesar el choque dos veces
        if (!other.CompareTag("Player")) return;

        yaColisiono = true;

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        if (GameManager.Instance != null)
        {
            if (type == ObstacleType.Explosivo)
            {
                // Barril explosivo: pérdida total de combustible -> game over inmediato
                GameManager.Instance.LoseFuel(9999f);
            }
            else
            {
                GameManager.Instance.LoseFuel(fuelPenalty);
            }
        }

        Destroy(gameObject);
    }
}
