using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class IntroScreen : MonoBehaviour
{
    [SerializeField] Image blackScreen;


    // Start is called before the first frame update
    void Awake()
    {
        blackScreen.gameObject.SetActive(true);

        Invoke(nameof(FadeOut), .2f);
    }

    void FadeOut()
    {
        blackScreen.DOColor(new Color(0, 0, 0, 0), 1f);
    }

}
