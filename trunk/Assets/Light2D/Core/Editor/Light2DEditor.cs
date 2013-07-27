using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects()]
[CustomEditor(typeof(Light2D))]
public class Light2DEditor : Editor
{
    float l_Radius = 0;
    int l_SweepSize = 0;
    float l_SweepStart = 0;
    int l_Layer = 1;
    Color l_Color = Color.white;
    Light2D.LightDetailSetting l_Detail = Light2D.LightDetailSetting.Normal;

    public override void OnInspectorGUI()
    {
        Light2D l = (Light2D)target;

        l.IsStatic = EditorGUILayout.Toggle(new GUIContent("Is Static", "Static lights will not cast shadows in 'Play' mode. They will however cast shadows in edit mode."), l.IsStatic);
        l_Detail = (Light2D.LightDetailSetting)EditorGUILayout.EnumPopup("Detail", l.LightDetail);
        l_Color = EditorGUILayout.ColorField("Light Color", l.LightColor);
        l.LightMaterial = (Material)EditorGUILayout.ObjectField("Material", l.LightMaterial, typeof(Material), false);

        l_Layer = EditorGUILayout.LayerField("Layer", l.ShadowLayer - 1);

        EditorGUILayout.Separator();

        l.useEvents = EditorGUILayout.Toggle("Enable Events", l.useEvents);
        l.allowHideInsideColliders = EditorGUILayout.Toggle("Hide In Colliders", l.allowHideInsideColliders);
        l.ignoreOptimizations = EditorGUILayout.Toggle("Continuous Update", l.ignoreOptimizations);

        EditorGUILayout.Separator();

        l_SweepStart = Mathf.Clamp(EditorGUILayout.FloatField("Sweep Start", l.SweepStart), -360, 360);
        l_SweepSize = (int)Mathf.Clamp(EditorGUILayout.IntField("Sweep Size", l.SweepSize), 0, 360);
        l_Radius = Mathf.Clamp(EditorGUILayout.FloatField("Light Radius", l.LightRadius), 0.02f, Mathf.Infinity);

        if (GUI.changed)
        {
            l.LightDetail = l_Detail;
            l.LightRadius = l_Radius;
            l.SweepSize = l_SweepSize;
            l.SweepStart = l_SweepStart;
            l.LightColor = l_Color;
            l.ShadowLayer = 1 << l_Layer;
            l.UpdateLight2D();
        }
    }

    void OnSceneGUI()
    {
        Light2D l = (Light2D)target;
        EditorUtility.SetSelectedWireframeHidden(l.renderer, true);

        Handles.color = Color.green;
        float widgetSize = Vector3.Distance(l.transform.position, SceneView.lastActiveSceneView.camera.transform.position) * 0.05f;
        float rad = (l.LightRadius / 2f);
        Handles.DrawWireDisc(l.transform.position, l.transform.forward, rad);
        l.LightRadius = Handles.ScaleValueHandle(l.LightRadius, l.transform.TransformPoint(Vector3.right * rad), Quaternion.identity, widgetSize, Handles.CubeCap, 1);

        Handles.color = Color.red;
        Vector3 sPos = l.transform.TransformDirection(Mathf.Cos(Mathf.Deg2Rad * l.SweepStart), Mathf.Sin(Mathf.Deg2Rad * l.SweepStart), 0);
        Handles.DrawWireArc(l.transform.position, l.transform.forward, sPos, l.SweepSize, (rad * 0.8f));
        l.SweepSize = Mathf.Clamp((int)Handles.ScaleValueHandle(l.SweepSize, l.transform.position + sPos * (rad * 0.8f), Quaternion.identity, widgetSize, Handles.SphereCap, 1), 0, 360);
    }
}
