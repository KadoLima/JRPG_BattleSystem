using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class IntroScreen : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] Image blackScreen;


    void Awake()
    {
        StartCoroutine(ShowContent_Coroutine());
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!GameManager.instance.GameStarted)
                CloseContent();
        }
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
        GameManager.instance.GameStarted = true;
        content.SetActive(false);
    }
}
