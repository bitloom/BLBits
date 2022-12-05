using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LocaliseImage))]
public class LocalisedImageInspector : Editor
{
    public static LocalisationData curData = null;

    private static bool foldout = false;

    private SerializedProperty defaultSprite;
    private SerializedProperty languageSprites;

    private void OnEnable()
    {
        if(curData == null)
        {
            curData = AssetDatabase.LoadAssetAtPath<LocalisationData>("Assets/Resources/GameData/LocalisationData.asset");
        }

        defaultSprite = serializedObject.FindProperty("defaultSprite");
        languageSprites = serializedObject.FindProperty("languageSprites");
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

            EditorGUILayout.PropertyField(defaultSprite);

            foldout = EditorGUILayout.Foldout(foldout, "Override Fonts");

            if (foldout)
            {
                if (languageSprites.arraySize != curData.languageCodes.Count)
                {
                    languageSprites.arraySize = curData.languageCodes.Count;
                }

                for (int i = 0; i < languageSprites.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(languageSprites.GetArrayElementAtIndex(i), new GUIContent(curData.languageCodes[i]), true);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
