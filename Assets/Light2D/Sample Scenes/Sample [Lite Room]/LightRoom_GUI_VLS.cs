using UnityEngine;
using System.Collections;

public class LightRoom_GUI_VLS : MonoBehaviour 
{
    void OnGUI()
    {
        Color bg = Camera.main.backgroundColor;

        float a = bg.a;
        GUILayout.BeginHorizontal(GUILayout.Width(300));
        GUILayout.Space(20);
        GUILayout.Label("Ambient Haze [Camera Alpha]");
        a = GUILayout.HorizontalSlider(a, 0, 1);
        GUILayout.EndHorizontal();

        float c = bg.r;
        GUILayout.BeginHorizontal(GUILayout.Width(300));
        GUILayout.Space(20);
        GUILayout.Label("Ambient Color [Camera Color]");
        c = GUILayout.HorizontalSlider(c, 0, 1);
        GUILayout.EndHorizontal();

        Camera.main.backgroundColor = new Color(c, c, c, a);
    }
}
