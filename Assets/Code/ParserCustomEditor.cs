using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StringBrainSO), true)]
public class ParserCustomEditor : Editor
{
    #region Draw Inspector
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
        if (GUILayout.Button("Find Duplicate Strings In list"))
        {
            ((StringBrainSO)target).ParseCustomList();
        }
        if (GUILayout.Button("Find Duplicate Strings In XML"))
        {
            ((StringBrainSO)target).ParseXmlFile();
        }
        EditorGUI.EndDisabledGroup();
    }
    #endregion
}
