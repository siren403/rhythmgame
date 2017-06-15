#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BitStrap;

[ExecuteInEditMode]
public class CGrid : MonoBehaviour
{
    public enum AxisType { X = 0, Y, Z }

    public AxisType GridAxis = AxisType.Z;

    public float Distance = 10.0f;
    public Vector3 Offset = Vector3.zero;


    [Button]
    public void Reposition()
    {
        Vector3 pos = Vector3.zero;
        pos += Offset;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).localPosition = pos;
            switch (GridAxis)
            {
                case AxisType.X:
                    pos.x += Distance;
                    break;
                case AxisType.Y:
                    pos.y += Distance;
                    break;
                case AxisType.Z:
                    pos.z += Distance;
                    break;
            }
        }
    }
}

#endif