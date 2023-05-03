using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class HighlightButton : MonoBehaviour
{
    Tweener tweener;
    [SerializeField] float scaleMultiplier = 1;

    public void Selected()
    {
        //Debug.LogWarning("ASDJFSDJFSDJF");
        tweener = transform.DOScale(1.075f * scaleMultiplier, .1f);
    }

    public void Deselected()
    {
        tweener.Kill();
        transform.DOScale(1f, 0f);
    }

}
