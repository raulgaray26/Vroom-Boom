using UnityEngine;
using System.Collections;

/// <summary>
/// Da feedback visual al jugador cuando choca con un obstáculo:
/// - Golpe normal: aplica el efecto de daño y vuelve a la normalidad después de "duration" segundos.
/// - Golpe fatal (barril explosivo): se llama con duration <= 0 y el efecto queda permanente,
///   ya que el juego termina ahí.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerDamageEffect : MonoBehaviour
{
    [Header("Opción A: cambiar de sprite")]
    public Sprite damagedSprite;

    [Header("Opción B: tinte de color")]
    public Color damageTintColor = new Color(1f, 0.35f, 0.35f); // rojo suave

    private SpriteRenderer sr;
    private Sprite normalSprite;
    private Color normalColor;
    private Coroutine revertCoroutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        normalSprite = sr.sprite;
        normalColor = sr.color;
    }

    /// <summary>
    /// Aplica el efecto de daño.
    /// duration > 0  → vuelve a la normalidad después de esos segundos (golpe normal).
    /// duration <= 0 → el daño queda permanente (golpe fatal / game over).
    /// </summary>
    public void TriggerDamage(float duration)
    {
        if (revertCoroutine != null) StopCoroutine(revertCoroutine);

        if (damagedSprite != null)
        {
            sr.sprite = damagedSprite;
        }
        else
        {
            sr.color = damageTintColor;
        }

        if (duration > 0f)
        {
            revertCoroutine = StartCoroutine(RevertirDespuesDe(duration));
        }
    }

    private IEnumerator RevertirDespuesDe(float duration)
    {
        yield return new WaitForSeconds(duration);
        sr.sprite = normalSprite;
        sr.color = normalColor;
    }
}
