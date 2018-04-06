using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AddMinimap))]
public class AddMinimapEditor : Editor 
{
    private string addString = "Add Minimap";
    private string clearString = "Clear Minimap";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var addMinimap = (AddMinimap)target;

        if (GUILayout.Button(addString))
            addMinimap.PutQuad();

        if (GUILayout.Button(clearString))
            addMinimap.ClearMinimap();
    }

    [CustomPropertyDrawer(typeof(AddMinimap.SingleUnityLayer))]
    public class SingleUnityLayerPropertyDrawer : PropertyDrawer 
    {
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            EditorGUI.BeginProperty(_position, GUIContent.none, _property);
            SerializedProperty layerIndex = _property.FindPropertyRelative("m_LayerIndex");
            _position = EditorGUI.PrefixLabel(_position, GUIUtility.GetControlID(FocusType.Passive), _label);
            if (layerIndex != null)
            {
                layerIndex.intValue = EditorGUI.LayerField(_position, layerIndex.intValue);
            }
            EditorGUI.EndProperty( );
        }
    }
}
