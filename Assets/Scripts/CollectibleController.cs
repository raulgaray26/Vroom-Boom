using UnityEngine;

/// <summary>
/// Controla el comportamiento de un objeto coleccionable (bidón de combustible o moneda):
/// - Se mueve hacia abajo simulando el avance del carro.
/// - Se destruye si sale de la pantalla sin ser recolectado (evita fugas de memoria).
/// - Al chocar con el jugador: desaparece, suma puntos y (si es combustible) rellena la barra.
/// </summary>
public class CollectibleController : MonoBehaviour
{
    public enum CollectibleType { Fuel, Coin }

    [Header("Configuración")]
    public CollectibleType type = CollectibleType.Coin;

    [Tooltip("Puntos que suma al marcador al recolectarlo.")]
    public int scoreValue = 10;

    [Tooltip("Cantidad de combustible que rellena (solo aplica si Type = Fuel).")]
    public float fuelAmount = 25f;

    [Header("Límite inferior de pantalla")]
    [Tooltip("Coordenada Y por debajo de la cual el objeto se autodestruye si nadie lo recolectó.")]
    public float destroyY = -6f;

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
        if (!other.CompareTag("Player")) return;

        // Suma puntos siempre
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);

            if (type == CollectibleType.Fuel)
            {
                GameManager.Instance.AddFuel(fuelAmount);
            }
        }
        Destroy(gameObject);
    }
}
