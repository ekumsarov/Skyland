using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lodkod;

public class ObjectTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public TooltipObject objectMode;
    [SerializeField] float _showTime = 0f;

    public string Text;
    public string TooltipText
    {
        get { return Text; }
        set { Text = value; }
    }

    SkyObject component;

    

    void ShowToolTip()
    {
        if (component == null)
            this.component = gameObject.GetComponent<SkyObject>();

        if(Text == null || Text.Equals(""))
        {
            Showed = false;

            return;
        }

        Showed = true;
        UIM.ShowTooltip(component, TooltipFit.Auto, TooltipTimeMode.ObjectManagment, TooltipFillMode.Instantly, objectMode, Text, lSize:30);
    }

    public void HardHide()
    {
        if (component == null)
            this.component = gameObject.GetComponent<SkyObject>();

        Showed = false;
        UIM.HideTooltip(component);
    }

    void HideTooltip()
    {
        if (component == null)
            this.component = gameObject.GetComponent<SkyObject>();

        Showed = false;
        UIM.HideTooltip(component);
    }

    bool Entered = false;
    bool Showed = false;
    public void OnPointerEnter(PointerEventData data)
    {
        if (Entered || Text.Equals(""))
            return;

        Entered = true;
        if (_showTime <= 0)
            ShowToolTip();
        else
            StartCoroutine(CheckTime());
    }

    public void OnPointerExit(PointerEventData data)
    {
        Entered = false;
        StopAllCoroutines();

        if (Showed)
            HideTooltip();
    }

    void Awake()
    {
        if (component == null)
            this.component = gameObject.GetComponent<SkyObject>();
    }

    IEnumerator CheckTime()
    {
        float currentTimer = _showTime;

        while(currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
            yield return null;
        }

        ShowToolTip();
        yield return null;
    }
}
