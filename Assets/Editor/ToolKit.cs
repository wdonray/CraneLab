using UnityEditor;
using UnityEngine;

public class ToolKit : EditorWindow
{
    [MenuItem("Window/Tools/ToolKit")]
    public static void ShowWindow()
    {
        GetWindow<ToolKit>("ToolKit");
    }

    private void OnGUI()
    {
        GUILayout.Label("Game Tools", EditorStyles.boldLabel);


        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (GUILayout.Button("Make Blue"))
        {

        }


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Make Red"))
        {

        }
        if (GUILayout.Button("Make Green"))
        {

        }
        GUILayout.EndHorizontal();


        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Spawning", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Spawn Sphere"))
        {

        }
        if (GUILayout.Button("Spawn Cube"))
        {

        }
        GUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }
}
