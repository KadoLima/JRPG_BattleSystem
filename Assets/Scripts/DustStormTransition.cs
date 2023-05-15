using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DustStormTransition : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void PlayParticles()
    {
        ParticleSystem _dustStorm = GetComponent<ParticleSystem>();

        _dustStorm.Play();
        _dustStorm.transform.DOMoveX(0, 7f).SetEase(Ease.OutQuint);
    }
}
