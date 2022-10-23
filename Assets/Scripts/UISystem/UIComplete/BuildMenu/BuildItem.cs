using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class BuildItem : UIItem
{
    [SerializeField] UIImage _icon;

    public BuildInfo BindedInfo;

    public void BindBuildInfo(BuildInfo info)
    {
        this.BindedInfo = info;
        _icon.Image = info.Icon;
    }
}
