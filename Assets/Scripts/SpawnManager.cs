using System.Collections;
using UnityEngine;

/// <summary>
/// Genera coleccionables y obstáculos periódicamente en la parte superior de la pantalla,
/// y controla la progresión de dificultad asignada al Integrante 1:
/// la velocidad de scroll y la frecuencia de spawn aumentan con el tiempo transcurrido.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject[] collectiblePrefabs;
    public GameObject[] obstaclePrefabs;

    // La zona de aparición ahora se toma de RoadConfig (compartida con PlayerController,
    // CollectibleController y ObstacleController) en vez de tener sus propios campos aquí.

    [Header("Frecuencia de spawn (dificultad)")]
    [Tooltip("Segundos entre cada spawn al inicio del juego.")]
    public float spawnIntervalInicial = 1.4f;

    [Tooltip("Segundos mínimos entre spawns (nunca baja de esto, para que siga siendo jugable).")]
    public float spawnIntervalMinimo = 0.45f;

    [Tooltip("Cuánto disminuye el intervalo de spawn por segundo transcurrido de partida.")]
    public float reduccionIntervaloPorSegundo = 0.01f;

    [Header("Velocidad de scroll (dificultad)")]
    [Tooltip("Velocidad inicial a la que se mueven hacia abajo los objetos.")]
    public float scrollSpeedInicial = 3.5f;

    [Tooltip("Velocidad máxima de scroll (tope para que no se vuelva imposible).")]
    public float scrollSpeedMaximo = 9f;

    [Tooltip("Cuánto aumenta la velocidad de scroll por segundo transcurrido de partida.")]
    public float aumentoVelocidadPorSegundo = 0.05f;

    [Header("Probabilidad")]
    [Range(0f, 1f)]
    [Tooltip("Probabilidad de que el próximo spawn sea un coleccionable en vez de un obstáculo.")]
    public float probabilidadCollectible = 0.45f;

    private float tiempoTranscurrido = 0f;
    private float currentSpawnInterval;
    private float currentScrollSpeed;
    private bool spawnActivo = false;

    void Awake()
    {
        // Patrón Singleton simple para que CollectibleController y ObstacleController
        // puedan leer la velocidad actual de scroll con SpawnManager.Instance.GetScrollSpeed()
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentSpawnInterval = spawnIntervalInicial;
        currentScrollSpeed = scrollSpeedInicial;
    }

    void Start()
    {
        IniciarSpawn();
    }

    void Update()
    {
        if (!spawnActivo) return;

        tiempoTranscurrido += Time.deltaTime;

        // Dificultad: la velocidad sube con el tiempo hasta el máximo
        currentScrollSpeed = Mathf.Min(
            scrollSpeedMaximo,
            scrollSpeedInicial + tiempoTranscurrido * aumentoVelocidadPorSegundo
        );

        // Dificultad: el intervalo entre spawns baja con el tiempo hasta el mínimo
        currentSpawnInterval = Mathf.Max(
            spawnIntervalMinimo,
            spawnIntervalInicial - tiempoTranscurrido * reduccionIntervaloPorSegundo
        );
    }

    /// <summary>Llamar para empezar a generar objetos (al arrancar la partida).</summary>
    public void IniciarSpawn()
    {
        spawnActivo = true;
        tiempoTranscurrido = 0f;
        currentSpawnInterval = spawnIntervalInicial;
        currentScrollSpeed = scrollSpeedInicial;
        StopAllCoroutines();
        StartCoroutine(SpawnLoop());
    }

    /// <summary>Llamar para detener la generación de objetos (Game Over).</summary>
    public void DetenerSpawn()
    {
        spawnActivo = false;
        StopAllCoroutines();
    }

    private IEnumerator SpawnLoop()
    {
        while (spawnActivo)
        {
            SpawnObjeto();
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    private void SpawnObjeto()
    {
        bool esCollectible = Random.value < probabilidadCollectible;
        GameObject[] fuente = esCollectible ? collectiblePrefabs : obstaclePrefabs;

        if (fuente == null || fuente.Length == 0) return;

        GameObject prefab = fuente[Random.Range(0, fuente.Length)];
        float x = Random.Range(RoadConfig.MinX, RoadConfig.MaxX);
        Vector3 posicion = new Vector3(x, RoadConfig.SpawnY, 0f);

        Instantiate(prefab, posicion, Quaternion.identity);
    }

    /// <summary>
    /// Usado por CollectibleController y ObstacleController para moverse todos
    /// a la misma velocidad, que aumenta con la dificultad.
    /// </summary>
    public float GetScrollSpeed()
    {
        return currentScrollSpeed;
    }
}
