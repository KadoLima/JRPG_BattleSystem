using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SpellBehaviour : MonoBehaviour
{
    public enum SpellType
    {
        PROJECTILE,
        ONTARGET_GROUND,
        ONTARGET_SKY,
        NULL
    }

    [Header("Anim")]
    [SerializeField] Animator myAnim;

    [Header("Appearance settings")]
    [SerializeField] float rotateSpeed = 1;
    [SerializeField] float additionalWidth = 0;
    [SerializeField] float finalScale = 0.1f;
    [SerializeField] float projectileLifetime = 0.5f;
    [SerializeField] SpellType spellType;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Execute(Vector3 spawnPoint, CharacterBehaviour target)
    {
        if (projectileLifetime <= 0)
        {
            Debug.LogError("PROJECTILE LIFETIME NOT SPECIFIED");
            return;
        }

        transform.position = spawnPoint;

        switch (spellType)
        {
            case SpellType.PROJECTILE:
                if (myAnim) myAnim.enabled = true;
                transform.DOMove(target.GetComponentInChildren<SpriteEffects>().SpotsToGetHit[0].position, projectileLifetime - .2f).SetEase(Ease.InOutExpo);
                break;
            case SpellType.ONTARGET_GROUND:
                transform.DOMove(target.GetComponentInChildren<SpriteEffects>().SpotsToGetHit[1].position, 0);
                if (myAnim) myAnim.enabled = true;

                break;
            case SpellType.ONTARGET_SKY:
                break;
            case SpellType.NULL:
                break;
        }

        FadeOutAndDestroy(projectileLifetime);
    }

    private void OnEnable()
    {
        this.transform.localScale = Vector3.zero;

        this.transform.DOScale(new Vector2(finalScale+additionalWidth, finalScale), .5f);
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, -rotateSpeed));
    }

    public void FadeOutAndDestroy(float delay = 0)
    {
        StartCoroutine(FadeOutDestroy_Coroutine(delay));
    }

    IEnumerator FadeOutDestroy_Coroutine(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        float _fadeOutTime = .25f;
        spriteRenderer.DOColor(new Color(1, 1, 1, 0), _fadeOutTime);
        Destroy(this.gameObject, _fadeOutTime + .5f);
    }
}
