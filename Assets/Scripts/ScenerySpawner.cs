using System.Collections;
using UnityEngine;

/// <summary>
/// Genera elementos decorativos (árboles, postes, etc.) en las franjas
/// laterales fuera de la carretera, para reforzar la sensación de movimiento.
/// No tienen colisión ni afectan el gameplay.
/// </summary>
public class ScenerySpawner : MonoBehaviour
{
    [Header("Prefabs de decoración")]
    public GameObject[] sceneryPrefabs;

    [Header("Franjas laterales (fuera de la carretera)")]
    public float leftMinX = -8f;
    public float leftMaxX = -5.2f;
    public float rightMinX = 5.2f;
    public float rightMaxX = 8f;

    [Header("Frecuencia de spawn")]
    public float spawnInterval = 0.6f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnDecoracion();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnDecoracion()
    {
        if (sceneryPrefabs == null || sceneryPrefabs.Length == 0) return;

        // Elige aleatoriamente lado izquierdo o derecho
        bool ladoIzquierdo = Random.value < 0.5f;
        float x = ladoIzquierdo
            ? Random.Range(leftMinX, leftMaxX)
            : Random.Range(rightMinX, rightMaxX);

        GameObject prefab = sceneryPrefabs[Random.Range(0, sceneryPrefabs.Length)];
        Vector3 pos = new Vector3(x, RoadConfig.SpawnY, 0f);
        Instantiate(prefab, pos, Quaternion.identity);
    }
}