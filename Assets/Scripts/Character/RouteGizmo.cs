using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RouteGizmo : MonoBehaviour
{
    public LineRenderer Line;
    public Vector3[] InitialState = new Vector3[1];
    public float SmoothingLength = 2f;
    public int SmoothingSections = 10;
}
