using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FunkyCode;
using FunkyCode.LightingSettings;
using FunkyCode.LightSettings;

public class Sun : MonoBehaviour
{
    [SerializeField] float rightLimitX;
    [SerializeField] float timePeriod = 120f;
    [SerializeField] float alphaTargetValue = .77f;
    [SerializeField] Light2D light2D;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        yield return new WaitUntil(() => GameManager.instance.gameStarted == true);
        transform.DOMoveX(rightLimitX, timePeriod).SetEase(Ease.Linear);

        float _timeToMidScreen = timePeriod / 2;
        float startingValue = light2D.size;
        float finalValue = 100;
        float incrementPerSecond = (finalValue - startingValue) / _timeToMidScreen;
        float currentTime = 0;

        float _colorAlphaStartingValue = light2D.color.a;
        float _alphaIncrementPerSecond = (alphaTargetValue - _colorAlphaStartingValue) / _timeToMidScreen;

        while (currentTime < _timeToMidScreen)
        {
            startingValue += incrementPerSecond * Time.deltaTime;
            currentTime+= Time.deltaTime;
            light2D.size = startingValue;

            _colorAlphaStartingValue += _alphaIncrementPerSecond * Time.deltaTime;
            light2D.color = new Color(light2D.color.r, light2D.color.g, light2D.color.b, _colorAlphaStartingValue);
            yield return null;
        }

        yield return new WaitForSeconds(.25f);

        startingValue = finalValue;
        finalValue = 50;
        incrementPerSecond = (finalValue - startingValue) / _timeToMidScreen;
        currentTime = 0;

        _colorAlphaStartingValue = alphaTargetValue;
        alphaTargetValue = .6f;
        _alphaIncrementPerSecond = (alphaTargetValue - _colorAlphaStartingValue) / _timeToMidScreen;

        while (currentTime < _timeToMidScreen)
        {
            startingValue += incrementPerSecond * Time.deltaTime;
            currentTime += Time.deltaTime;
            light2D.size = startingValue;

            _colorAlphaStartingValue += _alphaIncrementPerSecond * Time.deltaTime;
            light2D.color = new Color(light2D.color.r, light2D.color.g, light2D.color.b, _colorAlphaStartingValue);
            yield return null;
        }
    }
}
