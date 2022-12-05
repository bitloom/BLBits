using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LocalisedFontManager))]
public class LocalisedFontManagerInspector : Editor
{
    public static LocalisationData curData = null;

    private SerializedProperty defaultFont;
    private SerializedProperty fonts;

    private void OnEnable()
    {
        if(curData == null)
        {
            curData = AssetDatabase.LoadAssetAtPath<LocalisationData>("Assets/Resources/GameData/LocalisationData.asset");
        }

        defaultFont = serializedObject.FindProperty("defaultFont");
        fonts = serializedObject.FindProperty("fonts");
    }

    public override void OnInspectorGUI()
    {
        if (curData == null || curData.languageCodes == null || curData.languageCodes.Count == 0)
        {
            base.OnInspectorGUI();
        }
        else
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(defaultFont);

            EditorGUILayout.LabelField("Override Fonts", EditorStyles.boldLabel);

            if(fonts.arraySize != curData.languageCodes.Count)
            {
                fonts.arraySize = curData.languageCodes.Count;
            }

            for(int i = 0; i < fonts.arraySize; i++)
            {
                EditorGUILayout.PropertyField(fonts.GetArrayElementAtIndex(i), new GUIContent(curData.languageCodes[i]), true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
