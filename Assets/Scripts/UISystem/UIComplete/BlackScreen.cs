using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;
using System;

public class BlackScreen : MenuEx {
    
    public UIImage BlackImage;
    GameEvent gEvent;
    Action CallFunc;
    bool fadeIn;
    float Speed;
    float currentSpeed;

    public void StartFade(bool _in, GameEvent ev = null, Action callF = null, float speed = 0.8f)
    {
        this.fadeIn = _in;
        this.gEvent = ev;
        this.CallFunc = callF;
        this.Speed = speed;

        if(_in)
        {
            this.BlackImage.SetAlpha(0);
            this.currentSpeed = 0f;
        }
        else
        {
            this.BlackImage.SetAlpha(1);
            this.currentSpeed = this.Speed;
        }

        this.StopAllCoroutines();
        this.gameObject.SetActive(true);
        StartCoroutine("Fading");
    }

    IEnumerator Fading()
    {
        if (this.fadeIn)
        {
            while (BlackImage.Color.a < 1)
            {
                this.currentSpeed += Time.deltaTime;
                BlackImage.SetAlpha(this.currentSpeed/this.Speed);
                yield return null;
            }
            BlackImage.SetAlpha(1);
        }
        else
        {
            while (BlackImage.Color.a > 0)
            {
                this.currentSpeed -= Time.deltaTime;
                BlackImage.SetAlpha(this.currentSpeed / this.Speed);
                yield return null;
            }
            BlackImage.SetAlpha(0);
        }

        CompleteFade();
    }

    public void CompleteFade()
    {
        if (!this.fadeIn)
            this.gameObject.SetActive(false);

        this.StopAllCoroutines();
        gEvent?.End();
        CallFunc?.Invoke();
    }

    protected override void Show()
    {

        this.PrepareChilds();
        this.BlackImage.SetAlpha(1);
        this.gameObject.SetActive(true);

        
        //this.FadeIn();
    }

    protected override void Hide()
    {
        this.PrepareChilds();
        this.BlackImage.SetAlpha(0);
        this.gameObject.SetActive(false);
        
        //this.FadeOut(1);
    }
}
