using System;
using UnityEngine;

public class MyGizmoHandler : MonoBehaviour
{
    public Action DrawGizmos;
    public Action DrawGizmosSelected;

    private void OnDrawGizmos()
    {
        DrawGizmos?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmosSelected?.Invoke();
    }
}