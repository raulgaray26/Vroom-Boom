using UnityEngine;

/// <summary>
/// Controla el movimiento del carro del jugador.
/// - Movimiento horizontal (cambiar de carril) y vertical (acelerar/frenar) con WASD o flechas.
/// - El carro no puede salir del área visible de la cámara (clamp).
/// - Activa un booleano "IsMoving" en el Animator para animación idle/movimiento.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Velocidad")]
    [Tooltip("Velocidad de movimiento horizontal (cambiar de carril).")]
    public float horizontalSpeed = 8f;

    [Tooltip("Velocidad de movimiento vertical (acelerar/frenar).")]
    public float verticalSpeed = 6f;

    [Header("Límites de pantalla (clamp)")]
    [Tooltip("Margen en unidades de mundo para que el carro no quede pegado al borde exacto.")]
    public float screenPadding = 0.4f;

    [Tooltip("Límite vertical superior relativo (0 = centro de cámara, 1 = borde superior).")]
    [Range(0f, 1f)]
    public float maxViewportY = 0.85f;

    [Tooltip("Límite vertical inferior relativo (0 = centro de cámara, -1 no aplica; usa 0..1 desde abajo).")]
    [Range(0f, 1f)]
    public float minViewportY = 0.1f;

    private Rigidbody2D rb;
    private Animator animator;
    private Camera mainCamera;

    private float minX, maxX, minY, maxY;

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
        // Leemos el input en Update (más preciso para input),
        // pero movemos el Rigidbody2D en FixedUpdate.
        float inputX = Input.GetAxisRaw("Horizontal"); // A/D o flechas izquierda/derecha
        float inputY = Input.GetAxisRaw("Vertical");   // W/S o flechas arriba/abajo

        moveInput = new Vector2(inputX, inputY);

        if (animator != null)
        {
            bool isMoving = moveInput.sqrMagnitude > 0.01f;
            animator.SetBool("IsMoving", isMoving);
        }
    }

    private Vector2 moveInput;

    void FixedUpdate()
    {
        Vector2 desiredPos = rb.position + moveInput.normalized * new Vector2(horizontalSpeed, verticalSpeed) * Time.fixedDeltaTime;

        // Clamp para que el carro no salga del área visible de la cámara
        desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
        desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);

        rb.MovePosition(desiredPos);
    }

    /// <summary>
    /// Recalcula los límites de mundo según el tamaño actual de la cámara.
    /// Se llama en Start(); si cambias el tamaño de cámara en tiempo de ejecución, puedes
    /// volver a llamarla manualmente.
    /// </summary>
    public void RecalcularLimites()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, mainCamera.nearClipPlane));

        minX = bottomLeft.x + screenPadding;
        maxX = topRight.x - screenPadding;

        // Usamos viewport (0..1) para dejar al jugador moverse solo en la franja inferior de la pantalla,
        // que es el estilo típico de "carretera infinita" (el carro se queda abajo, el fondo se mueve).
        Vector3 minPointViewport = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, minViewportY, mainCamera.nearClipPlane));
        Vector3 maxPointViewport = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, maxViewportY, mainCamera.nearClipPlane));

        minY = minPointViewport.y + screenPadding * 0.25f;
        maxY = maxPointViewport.y - screenPadding * 0.25f;
    }
}
