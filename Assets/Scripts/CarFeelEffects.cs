using UnityEngine;

/// <summary>
/// Agrega sensación de movimiento al auto: humo de escape continuo, marcas de llanta
/// al girar fuerte, y una vibración sutil mientras se mueve. Las tres cosas son
/// independientes — puedes dejar cualquier campo vacío/desactivado si no lo quieres usar.
///
/// CÓMO USARLO:
/// 1. Agrega este script al GameObject "Player" (junto a PlayerController).
/// 2. Arma los hijos que necesites (ver guía) y arrástralos a los campos del Inspector.
/// </summary>
public class CarFeelEffects : MonoBehaviour
{
    [Header("Humo de escape (opcional)")]
    [Tooltip("Particle System hijo, ubicado en la parte trasera del auto.")]
    public ParticleSystem exhaustSmoke;

    [Tooltip("Cuántas partículas de humo por segundo cuando el auto está quieto (idle).")]
    public float exhaustRateIdle = 3f;

    [Tooltip("Cuántas partículas de humo por segundo cuando el auto se está moviendo.")]
    public float exhaustRateMoving = 14f;

    [Header("Marcas de llanta al derrapar (opcional)")]
    [Tooltip("TrailRenderer de la rueda trasera izquierda.")]
    public TrailRenderer tireTrailLeft;

    [Tooltip("TrailRenderer de la rueda trasera derecha.")]
    public TrailRenderer tireTrailRight;

    [Range(0f, 1f)]
    [Tooltip("Qué tan fuerte tienes que girar (input horizontal) para que aparezcan las marcas.")]
    public float turnThresholdForSkid = 0.6f;

    [Header("Vibración sutil (opcional)")]
    public bool enableVibration = true;

    [Tooltip("Qué tan notoria es la vibración (mientras más chico, más sutil).")]
    public float vibrationAmplitude = 0.015f;

    [Tooltip("Qué tan rápido vibra.")]
    public float vibrationSpeed = 22f;

    private Vector3 spriteBaseScale;
    private Transform spriteTransform;

    void Awake()
    {
        // La vibración se aplica sobre la escala del propio sprite, con una amplitud
        // muy chica — no afecta de forma perceptible al Collider2D ni a la jugabilidad.
        spriteTransform = transform;
        spriteBaseScale = spriteTransform.localScale;
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        bool estaMoviendo = Mathf.Abs(inputX) > 0.05f || Mathf.Abs(inputY) > 0.05f;

        ActualizarHumo(estaMoviendo);
        ActualizarMarcasDeLlanta(inputX);
        ActualizarVibracion(estaMoviendo);
    }

    private void ActualizarHumo(bool estaMoviendo)
    {
        if (exhaustSmoke == null) return;

        var emission = exhaustSmoke.emission;
        emission.rateOverTime = estaMoviendo ? exhaustRateMoving : exhaustRateIdle;

        if (!exhaustSmoke.isPlaying) exhaustSmoke.Play();
    }

    private void ActualizarMarcasDeLlanta(float inputX)
    {
        bool derrapando = Mathf.Abs(inputX) >= turnThresholdForSkid;

        if (tireTrailLeft != null) tireTrailLeft.emitting = derrapando;
        if (tireTrailRight != null) tireTrailRight.emitting = derrapando;
    }

    private void ActualizarVibracion(bool estaMoviendo)
    {
        if (!enableVibration)
        {
            spriteTransform.localScale = spriteBaseScale;
            return;
        }

        if (estaMoviendo)
        {
            float offset = Mathf.Sin(Time.time * vibrationSpeed) * vibrationAmplitude;
            spriteTransform.localScale = spriteBaseScale + new Vector3(offset, offset, 0f);
        }
        else
        {
            spriteTransform.localScale = spriteBaseScale;
        }
    }
}
