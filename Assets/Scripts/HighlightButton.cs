using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class HighlightButton : MonoBehaviour
{
    Tweener tweener;

    public void Selected()
    {
        tweener = transform.DOScale(1.075f, .1f);
    }

    public void Deselected()
    {
        tweener.Kill();
        transform.DOScale(1f, 0f);
    }

}
