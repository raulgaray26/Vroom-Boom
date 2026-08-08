using UnityEngine;

/// <summary>
/// Se coloca en el prefab de explosión. Se autodestruye cuando termina el efecto,
/// para no dejar GameObjects acumulados en la escena cada vez que hay un choque.
///
/// CÓMO USARLO:
/// 1. Arrástralo al GameObject de tu Particle System de explosión (ver guía para armarlo).
/// 2. Si "Lifetime" queda en 0, el script calcula automáticamente la duración real del
///    Particle System. Si le pones un valor mayor a 0, usa ese tiempo fijo en su lugar.
/// </summary>
public class ExplosionEffect : MonoBehaviour
{
    [Tooltip("Segundos antes de autodestruirse. Déjalo en 0 para calcularlo automático desde el Particle System.")]
    public float lifetime = 0f;

    void Start()
    {
        float delay = lifetime;

        if (delay <= 0f)
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();
            if (ps != null)
            {
                delay = ps.main.duration + ps.main.startLifetime.constantMax;
            }
            else
            {
                delay = 1f; // valor de respaldo si no hay Particle System
            }
        }

        Destroy(gameObject, delay);
    }
}
