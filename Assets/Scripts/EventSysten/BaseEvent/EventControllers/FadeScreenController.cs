using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;

public class FadeScreenController : MonoBehaviour {

    public Image FadeImage;
    public GameEvent gEvent;
    public Action CallFunc;

    public void StartFade(bool In, GameEvent ev = null, Action callF = null)
    {
        gEvent = ev;
        CallFunc = callF;

        if (In)
            StartCoroutine("FadeIn");
        else
            StartCoroutine("FadeOut");
    }

    public void FadeOutImmediately()
    {
        FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, 0f);
    }

    public void FadeInImmediately()
    {
        FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, 1.0f);
    }

    public IEnumerator FadeOut()
    {
        while (FadeImage.color.a > 0)
        {
            float delta = Time.deltaTime;
            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, FadeImage.color.a - delta);
            yield return null;
        }


        gEvent?.End();
        CallFunc?.Invoke();
    }

    public IEnumerator FadeIn()
    {
        while (FadeImage.color.a < 1)
        {
            float delta = Time.deltaTime;
            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, FadeImage.color.a + delta);
            yield return null;
        }

        gEvent?.End();
        CallFunc?.Invoke();
    }
}
