using UnityEngine;
using UnityEditor;
using System.Collections;

public class Light2DMenu : Editor
{
    [MenuItem("GameObject/Create Other/Light2D/RadialLight", false, 50)]
    public static void CreateNewRadialLight()
    {
        Light2D light = Light2D.Create(Vector3.zero, (Material)Resources.Load("RadialLightMaterial"), Color.blue);
        light.ShadowLayer = LayerMask.NameToLayer("Default");

        Selection.activeGameObject = light.gameObject;
    }

    [MenuItem("GameObject/Create Other/Light2D/SpotLight", false, 51)]
    public static void CreateNewSpotLight()
    {
        Light2D light = Light2D.Create(Vector3.zero, (Material)Resources.Load("RadialLightMaterial"), Color.green);
        light.SweepSize = 45;
        light.SweepStart = -110;
        light.LightDetail = Light2D.LightDetailSetting.VeryLow;
        light.ShadowLayer = LayerMask.NameToLayer("Default");

        Selection.activeGameObject = light.gameObject;
    }

    [MenuItem("GameObject/Create Other/Light2D/Online Help", false, 51)]
    public static void SeekHelp()
    {
        Application.OpenURL("http://www.reverieinteractive.com/light2d-documentation");
    }
}
