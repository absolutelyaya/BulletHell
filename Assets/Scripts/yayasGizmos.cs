using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class yayasGizmos
{
    public static void DrawArrow(Vector3 from, Vector3 to, float headAngle, float headLength)
    {
        Gizmos.DrawLine(from, to);
        Gizmos.DrawLine(to, to + Quaternion.LookRotation((to - from).normalized) * Quaternion.Euler(0, 180 + headAngle, 0) * new Vector3(0, 0, 1) * headLength);
        Gizmos.DrawLine(to, to + Quaternion.LookRotation((to - from).normalized) * Quaternion.Euler(0, 180 - headAngle, 0) * new Vector3(0, 0, 1) * headLength);
        Gizmos.DrawLine(to, to + Quaternion.LookRotation((to - from).normalized) * Quaternion.Euler(180 + headAngle, 0, 0) * new Vector3(0, 0, 1) * headLength);
        Gizmos.DrawLine(to, to + Quaternion.LookRotation((to - from).normalized) * Quaternion.Euler(180 - headAngle, 0, 0) * new Vector3(0, 0, 1) * headLength);
    }

    public static void DrawRing(Vector3 position, int elements, float baseDistance)
    {
        float distance = baseDistance + 0.45f + Mathf.Clamp(elements - 4, 0, Mathf.Infinity) * 0.125f * Mathf.Pow(1.00001f, elements) / 2;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(position, distance);
        Gizmos.color = Color.red;
        for (int i = 0; i < elements; i++)
        {
            Gizmos.DrawWireSphere(position + Quaternion.AngleAxis(360f / elements * i, Vector3.forward * distance) * new Vector3(0, 1, 0) * distance, 0.1f);
        }
    }
}
