using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class ShowTooltip : GameEvent
    {
        SkyObject obj;
        Vector3 target;

        TooltipFit fit;
        TooltipTimeMode timeMode;
        TooltipFillMode fillMode;
        TooltipObject objectMode;

        float exTime;

        int LengthSize;
        string Text;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "ShowTooltip";

            obj = null;
            if (node["ID"] != null)
                obj = GM.GetObject(node["ID"].Value);

            this.target = UIM.ScreenCenter;
            if (node["point"] != null)
                this.target = new Vector3(node["point"]["x"].AsFloat, node["point"]["y"].AsFloat, -10f);

            LengthSize = 45;
            if (node["LenghtSize"] != null)
                LengthSize = node["LenghtSize"].AsInt;

            exTime = 1.0f;
            if (node["exTime"] != null)
                exTime = node["exTime"].AsFloat;

            fit = TooltipFit.Auto;
            if (node["Fit"] != null)
                fit = (TooltipFit)Enum.Parse(typeof(TooltipFit), node["Fit"].Value);

            timeMode = TooltipTimeMode.Click;
            if (node["TimeMode"] != null)
                timeMode = (TooltipTimeMode)Enum.Parse(typeof(TooltipTimeMode), node["TimeMode"].Value);

            fillMode = TooltipFillMode.Type;
            if (node["FillMode"] != null)
                fillMode = (TooltipFillMode)Enum.Parse(typeof(TooltipFillMode), node["FillMode"].Value);

            objectMode = TooltipObject.Game;
            if (node["ObjectMode"] != null)
                objectMode = (TooltipObject)Enum.Parse(typeof(TooltipObject), node["ObjectMode"].Value);

            Text = "NoText";
            if (node["text"] != null)
                Text = node["text"].Value;

            if (target == null)
                Debug.LogError("NOT SET CAMERA TARGET");

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override void Start()
        {
            if (obj)
                UIM.ShowTooltip(obj, fit, timeMode, fillMode, objectMode, Text, this, null, exTime, LengthSize);
            else
                UIM.ShowTooltip(target, fit, timeMode, fillMode, objectMode, Text, this, null, exTime, LengthSize);
        }


        #region static
        public static ShowTooltip Create(Vector3 _point, string ID, float exTime = 1.0f, int lenghtSize = 45, TooltipFit fit = TooltipFit.Auto, 
            TooltipTimeMode timeMode = TooltipTimeMode.Click, TooltipFillMode fillMode = TooltipFillMode.Type, TooltipObject objectMode = TooltipObject.Game, string Text = "NoText")
        {
            ShowTooltip temp = new ShowTooltip();
            temp.ID = "ShowTooltip";

            temp.obj = GM.GetObject(ID);

            _point = UIM.ScreenCenter;
            if (_point != Vector3.zero)
                temp.target = new Vector3(_point.x, _point.y, -10f);

            temp.LengthSize = lenghtSize;
            temp.fit = fit;
            temp.timeMode = timeMode;
            temp.fillMode = fillMode;
            temp.objectMode = objectMode;
            temp.exTime = exTime;
            temp.Text = Text;

            return temp;
        }
        #endregion
    }
}