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

    // El límite inferior de pantalla ahora se toma de RoadConfig.DestroyY.

    [Header("Efecto opcional")]
    [Tooltip("Prefab de explosión/partícula que se instancia al chocar (opcional, puede quedar vacío).")]
    public GameObject explosionEffectPrefab;

    private bool yaColisiono = false;

    void Update()
    {
        float speed = SpawnManager.Instance != null ? SpawnManager.Instance.GetScrollSpeed() : 4f;
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < RoadConfig.DestroyY)
        {
            Destroy(gameObject);
        }
    }

    [Header("Daño visual al jugador")]
    [Tooltip("Segundos que dura el efecto de daño en el auto tras un golpe normal.")]
    public float duracionDañoTemporal = 1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (yaColisiono) return; // evita procesar el choque dos veces
        if (!other.CompareTag("Player")) return;

        yaColisiono = true;

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Daño visual en el auto
        PlayerDamageEffect damageEffect = other.GetComponent<PlayerDamageEffect>();
        if (damageEffect != null)
        {
            if (type == ObstacleType.Explosivo)
            {
                damageEffect.TriggerDamage(0f); // permanente, el juego termina aquí
            }
            else
            {
                damageEffect.TriggerDamage(duracionDañoTemporal); // temporal
            }
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
