using UnityEngine;

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
    public AudioClip sfxNivelCompletado;
    public AudioClip sfxGameOver;
    public AudioClip sfxVictoria;
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
        GameManager.OnLevelUp += ManejarNivelUp;
        GameManager.OnGameOverEvent += ManejarGameOver;
        GameManager.OnVictoryEvent += ManejarVictoria;
    }

    void OnDisable()
    {
        GameManager.OnGameStart -= ManejarInicio;
        GameManager.OnCollect -= ManejarRecolectar;
        GameManager.OnObstacleHit -= ManejarColision;
        GameManager.OnLevelUp -= ManejarNivelUp;
        GameManager.OnGameOverEvent -= ManejarGameOver;
        GameManager.OnVictoryEvent -= ManejarVictoria;
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
    private void ManejarNivelUp(int nuevoNivel) => PlaySFX(sfxNivelCompletado);

    private void ManejarGameOver(int score, int highScore, bool esNuevoRecord)
    {
        fuenteMusica.Stop();
        PlaySFX(sfxGameOver);
    }

    private void ManejarVictoria(int score, int highScore, bool esNuevoRecord)
    {
        fuenteMusica.Stop();
        PlaySFX(sfxVictoria);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || fuenteSFX == null) return;
        fuenteSFX.PlayOneShot(clip, volumenSFX);
    }
}