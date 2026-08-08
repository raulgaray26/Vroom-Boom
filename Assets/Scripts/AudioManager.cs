using UnityEngine;

/// <summary>
/// Maneja música de fondo y SFX escuchando los eventos estáticos de GameManager.
/// Ningún otro script (PlayerController, ObstacleController, etc.) necesita
/// llamarlo directamente.
///
/// CÓMO USARLO:
/// 1. Crea un GameObject vacío llamado "AudioManager" en la escena.
/// 2. Agrega este script y arrastra tus AudioClips en el Inspector.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Música de fondo")]
    public AudioClip musicaJuego;
    [Range(0f, 1f)] public float volumenMusica = 0.5f;

    [Header("Efectos de sonido")]
    public AudioClip sfxRecolectar;
    public AudioClip sfxColision;
    public AudioClip sfxInicio;
    public AudioClip sfxGameOver;
    [Range(0f, 1f)] public float volumenSFX = 0.8f;

    private AudioSource fuenteMusica;
    private AudioSource fuenteSFX;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        fuenteMusica = gameObject.AddComponent<AudioSource>();
        fuenteMusica.loop = true;
        fuenteMusica.playOnAwake = false;
        fuenteMusica.volume = volumenMusica;

        fuenteSFX = gameObject.AddComponent<AudioSource>();
        fuenteSFX.playOnAwake = false;
        fuenteSFX.volume = volumenSFX;
    }

    void OnEnable()
    {
        GameManager.OnGameStart += ManejarInicio;
        GameManager.OnCollect += ManejarRecolectar;
        GameManager.OnObstacleHit += ManejarColision;
        GameManager.OnGameOverEvent += ManejarGameOver;
    }

    void OnDisable()
    {
        GameManager.OnGameStart -= ManejarInicio;
        GameManager.OnCollect -= ManejarRecolectar;
        GameManager.OnObstacleHit -= ManejarColision;
        GameManager.OnGameOverEvent -= ManejarGameOver;
    }

    private void ManejarInicio()
    {
        PlaySFX(sfxInicio);
        if (musicaJuego != null)
        {
            fuenteMusica.clip = musicaJuego;
            fuenteMusica.Play();
        }
    }

    private void ManejarRecolectar() => PlaySFX(sfxRecolectar);
    private void ManejarColision() => PlaySFX(sfxColision);

    private void ManejarGameOver(int score, int highScore, bool esNuevoRecord)
    {
        fuenteMusica.Stop();
        PlaySFX(sfxGameOver);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || fuenteSFX == null) return;
        fuenteSFX.PlayOneShot(clip, volumenSFX);
    }
}