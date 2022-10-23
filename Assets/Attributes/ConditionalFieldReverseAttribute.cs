using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ConditionalFieldReverseAttribute : PropertyAttribute
{
    public string PropertyToCheck;

    public object CompareValue;

    public ConditionalFieldReverseAttribute(string propertyToCheck, object compareValue = null)
    {
        PropertyToCheck = propertyToCheck;
        CompareValue = compareValue;
    }
}