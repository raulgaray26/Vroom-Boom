using UnityEngine;

/// <summary>
/// Controla el movimiento del carro del jugador.
/// - Movimiento horizontal (cambiar de carril) y vertical (acelerar/frenar) con WASD o flechas.
/// - El carro no puede salir del área visible de la cámara EN VERTICAL, y no puede salir
///   de la franja de carretera (RoadConfig.MinX/MaxX) EN HORIZONTAL.
/// - Envía el input horizontal al Animator como el float "Steer" (-1..1) para que el
///   Blend Tree incline el sprite hacia el lado al que te mueves.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Velocidad")]
    [Tooltip("Velocidad de movimiento horizontal (cambiar de carril).")]
    public float horizontalSpeed = 8f;

    [Tooltip("Velocidad de movimiento vertical (acelerar/frenar).")]
    public float verticalSpeed = 6f;

    [Header("Límites verticales (clamp contra cámara)")]
    [Tooltip("Límite vertical superior relativo (0 = borde inferior de cámara, 1 = borde superior).")]
    [Range(0f, 1f)]
    public float maxViewportY = 0.85f;

    [Tooltip("Límite vertical inferior relativo.")]
    [Range(0f, 1f)]
    public float minViewportY = 0.1f;

    [Tooltip("Margen extra en unidades de mundo para el clamp vertical.")]
    public float verticalPadding = 0.4f;

    [Header("Límites horizontales (clamp contra carretera)")]
    [Tooltip("Margen para que el auto no quede pegado al borde exacto de la carretera.")]
    public float horizontalPadding = 0.3f;

    private Rigidbody2D rb;
    private Animator animator;
    private Camera mainCamera;

    private float minX, maxX, minY, maxY;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        RecalcularLimites();
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal"); // A/D o flechas izquierda/derecha
        float inputY = Input.GetAxisRaw("Vertical");   // W/S o flechas arriba/abajo

        moveInput = new Vector2(inputX, inputY);

        if (animator != null)
        {
            // Steer maneja el Blend Tree que inclina el sprite hacia el lado del giro.
            animator.SetFloat("Steer", inputX);
        }
    }

    void FixedUpdate()
    {
        Vector2 desiredPos = rb.position + moveInput.normalized * new Vector2(horizontalSpeed, verticalSpeed) * Time.fixedDeltaTime;

        // Horizontal: clamp contra el ancho REAL de la carretera (no contra toda la cámara),
        // así el auto no se mete en las franjas de fuera de carretera.
        desiredPos.x = Mathf.Clamp(desiredPos.x, RoadConfig.MinX + horizontalPadding, RoadConfig.MaxX - horizontalPadding);

        // Vertical: clamp contra la cámara, para que se quede en la franja inferior de pantalla.
        desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);

        rb.MovePosition(desiredPos);
    }

    public void RecalcularLimites()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector3 minPointViewport = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, minViewportY, mainCamera.nearClipPlane));
        Vector3 maxPointViewport = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, maxViewportY, mainCamera.nearClipPlane));

        minY = minPointViewport.y + verticalPadding * 0.25f;
        maxY = maxPointViewport.y - verticalPadding * 0.25f;
    }
}
