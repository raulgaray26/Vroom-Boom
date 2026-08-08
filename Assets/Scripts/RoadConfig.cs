/// <summary>
/// Constantes compartidas de la carretera. Así PlayerController, SpawnManager,
/// CollectibleController y ObstacleController usan siempre los mismos límites
/// y no hay que sincronizar números a mano en cada script.
///
/// Ajusta estos 4 valores para cambiar el ancho de tu carretera
/// o el tamaño de tu cámara (se aplican automáticamente).
/// </summary>
public static class RoadConfig
{
    public const float MinX = -4.8f;

    public const float MaxX = 4.8f;

    public const float SpawnY = 6f;

    public const float DestroyY = -6f;
}
