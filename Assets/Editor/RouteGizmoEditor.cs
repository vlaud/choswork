using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RouteGizmo))]
public class RouteGizmoEditor : Editor
{
    private RouteGizmo Route;

    private SerializedProperty Line;
    private SerializedProperty InitialState;
    private SerializedProperty SmoothingLength;
    private SerializedProperty SmoothingSections;

    private GUIContent UpdateInitialStateGUIContent = new GUIContent("Set Initial State");
    private GUIContent SmoothButtonGUIContent = new GUIContent("Smooth Path");
    private GUIContent RestoreDefaultGUIContent = new GUIContent("Restore Default Path");

    //private bool ExpandCurves = false;
    private BezierCurve[] Curves;

    private void OnEnable()
    {
        Route = (RouteGizmo)target;

        if (Route.Line == null)
        {
            Route.Line = Route.GetComponent<LineRenderer>();
        }
        Line = serializedObject.FindProperty("Line");
        InitialState = serializedObject.FindProperty("InitialState");
        SmoothingLength = serializedObject.FindProperty("SmoothingLength");
        SmoothingSections = serializedObject.FindProperty("SmoothingSections");

        EnsureCurvesMatchLineRendererPositions();
    }
    public override void OnInspectorGUI()
    {
        if (Route == null)
        {
            return;
        }
        EnsureCurvesMatchLineRendererPositions();

        EditorGUILayout.PropertyField(Line);
        EditorGUILayout.PropertyField(InitialState);
        EditorGUILayout.PropertyField(SmoothingLength);
        EditorGUILayout.PropertyField(SmoothingSections);

        if (GUILayout.Button(UpdateInitialStateGUIContent))
        {
            Route.InitialState = new Vector3[Route.Line.positionCount];
            Route.Line.GetPositions(Route.InitialState);
        }

        EditorGUILayout.BeginHorizontal();
        {
            GUI.enabled = Route.Line.positionCount >= 3;
            if (GUILayout.Button(SmoothButtonGUIContent))
            {
                SmoothPath();
            }

            bool lineRendererPathAndInitialStateAreSame = Route.Line.positionCount == Route.InitialState.Length;

            if (lineRendererPathAndInitialStateAreSame)
            {
                Vector3[] positions = new Vector3[Route.Line.positionCount];
                Route.Line.GetPositions(positions);

                lineRendererPathAndInitialStateAreSame = positions.SequenceEqual(Route.InitialState);
            }

            GUI.enabled = !lineRendererPathAndInitialStateAreSame;
            if (GUILayout.Button(RestoreDefaultGUIContent))
            {
                Route.Line.positionCount = Route.InitialState.Length;
                Route.Line.SetPositions(Route.InitialState);

                EnsureCurvesMatchLineRendererPositions();
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
    private void SmoothPath()
    {
        Route.Line.positionCount = Curves.Length * SmoothingSections.intValue;
        int index = 0;
        for (int i = 0; i < Curves.Length; i++)
        {
            Vector3[] segments = Curves[i].GetSegments(SmoothingSections.intValue);
            for (int j = 0; j < segments.Length; j++)
            {
                Route.Line.SetPosition(index, segments[j]);
                index++;
            }
        }

        // Reset values so inspector doesn't freeze if you use lots of smoothing sections
        SmoothingSections.intValue = 1;
        SmoothingLength.floatValue = 0;
        serializedObject.ApplyModifiedProperties();
    }
    private void OnSceneGUI()
    {
        if (Route.Line.positionCount < 3)
        {
            return;
        }
        EnsureCurvesMatchLineRendererPositions();

        for (int i = 0; i < Curves.Length; i++)
        {
            Vector3 position = Route.Line.GetPosition(i);
            Vector3 lastPosition = i == 0 ? Route.Line.GetPosition(0) : Route.Line.GetPosition(i - 1);
            Vector3 nextPosition = Route.Line.GetPosition(i + 1);

            Vector3 lastDirection = (position - lastPosition).normalized;
            Vector3 nextDirection = (nextPosition - position).normalized;

            Vector3 startTangent = (lastDirection + nextDirection) * SmoothingLength.floatValue;
            Vector3 endTangent = (nextDirection + lastDirection) * -1 * SmoothingLength.floatValue;

            Handles.color = Color.green;
            Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), position + startTangent, Quaternion.identity, 0.25f, EventType.Repaint);

            if (i != 0)
            {
                Handles.color = Color.blue;
                Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), nextPosition + endTangent, Quaternion.identity, 0.25f, EventType.Repaint);
            }

            Curves[i].Points[0] = position; // Start Position (P0)
            Curves[i].Points[1] = position + startTangent; // Start Tangent (P1)
            Curves[i].Points[2] = nextPosition + endTangent; // End Tangent (P2)
            Curves[i].Points[3] = nextPosition; // End Position (P3)
        }

        // Apply look-ahead for first curve and retroactively apply the end tangent
        {
            Vector3 nextDirection = (Curves[1].EndPosition - Curves[1].StartPosition).normalized;
            Vector3 lastDirection = (Curves[0].EndPosition - Curves[0].StartPosition).normalized;

            Curves[0].Points[2] = Curves[0].Points[3] +
                (nextDirection + lastDirection) * -1 * SmoothingLength.floatValue;

            Handles.color = Color.blue;
            Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), Curves[0].Points[2], Quaternion.identity, 0.25f, EventType.Repaint);
        }

        DrawSegments();
    }
    private void DrawSegments()
    {
        for (int i = 0; i < Curves.Length; i++)
        {
            Vector3[] segments = Curves[i].GetSegments(SmoothingSections.intValue);
            for (int j = 0; j < segments.Length - 1; j++)
            {
                Handles.color = Color.white;
                Handles.DrawLine(segments[j], segments[j + 1]);

                float color = (float)j / segments.Length;
                Handles.color = new Color(color, color, color);
                Handles.Label(segments[j], $"C{i} S{j}");
                Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), segments[j], Quaternion.identity, 0.05f, EventType.Repaint);
            }

            Handles.color = Color.white;
            Handles.Label(segments[segments.Length - 1], $"C{i} S{segments.Length - 1}");
            Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), segments[segments.Length - 1], Quaternion.identity, 0.05f, EventType.Repaint);

            Handles.DrawLine(segments[segments.Length - 1], Curves[i].EndPosition);
        }
    }
    private void EnsureCurvesMatchLineRendererPositions()
    {
        if (Curves == null || Curves.Length != Route.Line.positionCount - 1)
        {
            Curves = new BezierCurve[Route.Line.positionCount - 1];
            for (int i = 0; i < Curves.Length; i++)
            {
                Curves[i] = new BezierCurve();
            }
        }
    }
}
