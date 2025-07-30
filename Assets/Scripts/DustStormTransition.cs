using UnityEngine;
using DG.Tweening;

public class DustStormTransition : MonoBehaviour
{
    private ParticleSystem particles;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        particles = GetComponent<ParticleSystem>();
    }

    public void PlayParticles()
    {
        particles.Play();
        particles.transform.DOMoveX(0, 7f).SetEase(Ease.OutQuint);
    }
}
