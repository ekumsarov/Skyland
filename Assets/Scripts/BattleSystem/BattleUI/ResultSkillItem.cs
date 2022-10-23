using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class ResultSkillItem : UIItem
{
    public UIImage skillIcon;
    public SimpleText skillValue;

    int _result;

    public override void Setting()
    {
        base.Setting();

        this.skillIcon.Visible = true;
        this.skillValue.Visible = true;
    }

    public void Setup(SkillCheckObject skill)
    {
        this.skillIcon.Image = SkillObject.SkillIcon(skill.Skill);
        this.skillValue.Text = skill.Complex.ToString();
    }

    public void SetValue(int value)
    {
        this.skillValue.Text = value.ToString();
    }

    public void StartAnimation(int finalResult)
    {
        _result = finalResult;
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        float timer = 1.5f;
        while(timer > 0)
        {
            this.skillValue.Text = string.Format("{0}", (int)UnityEngine.Random.Range(0, 100));
            timer -= Time.deltaTime;
            yield return null;
        }

        this.skillValue.Text = _result.ToString();
    }

    public void CountDownAnimation()
    {
        StartCoroutine(CountAnimation());
    }

    IEnumerator CountAnimation()
    {
        float timer = 0.5f;
        while (timer > 0)
        {
            this.skillValue.Text = string.Format("{0}", (int)((timer / 0.5f) * _result));
            timer -= Time.deltaTime;
            yield return null;
        }

        this.skillValue.Text = "0";
    }
}