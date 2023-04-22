using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class TweenExtensions
{
    public const float ANIMATION_TIME = 0.5f;

    public static Tweener DOMoveRect(this Transform transform, Vector2 to, float duration = ANIMATION_TIME)
    {
        RectTransform tr = (RectTransform)transform;
        return DOTween.To(() => tr.anchoredPosition, x => tr.anchoredPosition = x, to, duration);
    }

    public static Tweener DOMoveRectY(this Transform transform, float to, float duration = ANIMATION_TIME)
    {
        RectTransform tr = (RectTransform)transform;
        return DOTween.To(() => tr.anchoredPosition.y, y => tr.anchoredPosition = new Vector2(tr.anchoredPosition.x, y), to, duration);
    }

    public static Tweener DOMoveRectX(this Transform transform, float to, float duration = ANIMATION_TIME)
    {
        RectTransform tr = (RectTransform)transform;
        return DOTween.To(() => tr.anchoredPosition.x, x => tr.anchoredPosition = new Vector2(x, tr.anchoredPosition.y), to, duration);
    }
}
