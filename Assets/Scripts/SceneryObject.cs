using UnityEngine;

/// <summary>
/// Mueve un elemento decorativo hacia abajo y lo destruye al salir de pantalla.
/// </summary>
public class SceneryObject : MonoBehaviour
{
    void Update()
    {
        float speed = SpawnManager.Instance != null ? SpawnManager.Instance.GetScrollSpeed() : 4f;
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < RoadConfig.DestroyY)
        {
            Destroy(gameObject);
        }
    }
}