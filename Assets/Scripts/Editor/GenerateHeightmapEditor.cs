using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateHeightmap))]
public class GenerateHeightmapEditor : Editor 
{
    private string generateString = "GenerateHeightmap";
    private Editor settingsEditor;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        var settings = serializedObject.FindProperty("GenSettings");
        if (settings != null)
        {
            CreateCachedEditor(settings.objectReferenceValue, null, ref settingsEditor);
            if (settingsEditor != null)
                settingsEditor.OnInspectorGUI();
        }

        var addHeightmap = (GenerateHeightmap)target;

        if (GUILayout.Button(generateString))
            addHeightmap.Generate();
    }
        
}
