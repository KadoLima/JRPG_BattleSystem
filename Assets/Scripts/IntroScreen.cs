using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IntroScreen : MonoBehaviour
{
    //[SerializeField] Image blackScreen;
    [SerializeField] GameObject content;
    [SerializeField] Image blackScreen;
    //[SerializeField] GameObject okButton;


    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(ShowContent_Coroutine());
    }

    IEnumerator ShowContent_Coroutine()
    {
        blackScreen.gameObject.SetActive(true);
        content.SetActive(true);
        yield return new WaitForSeconds(1);

        float _blackFadeTime = .25f;
        blackScreen.DOFade(0, _blackFadeTime);
        yield return new WaitForSeconds(_blackFadeTime-0.2f);
        Transform _panel = content.transform.GetChild(1);
        _panel.GetComponent<RectTransform>().DOPunchScale(Vector3.one * 0.1f, .2f);

        blackScreen.gameObject.SetActive(false);
    }

    public void CloseContent()
    {
        GameManager.gameStarted = true;
        content.SetActive(false);
    }

    //void FadeOut()
    //{
        //blackScreen.DOColor(new Color(0, 0, 0, 0), 1f);
    //}

}
