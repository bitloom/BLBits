using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomPropertyDrawer(typeof(LocalisedTextAttribute))]
[CanEditMultipleObjects]
public class LocalisedTextDrawer : PropertyDrawer
{
    private static LocalisationData data;    
    private List<string> keys = new List<string>();

    void Awake()
    {
        SetupKeys();
        //curKeyIndex = FindCurIndex();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (keys.Count == 0)
        {
            SetupKeys();
        }

        int curKeyIndex = FindCurIndex(property.stringValue);

        EditorGUI.BeginChangeCheck();

        curKeyIndex = EditorGUI.Popup(position, "Text Key", curKeyIndex, keys.ToArray());

        if (EditorGUI.EndChangeCheck() && keys.Count > 0)
        {
            property.stringValue = keys[curKeyIndex];
        }

        EditorGUI.EndProperty();
    }

    void SetupKeys()
    {        
        string folderPath = "Assets/Resources/GameData";

#if DEMO_BUILD
        data = AssetDatabase.LoadAssetAtPath<LocalisationData>(Path.Combine(folderPath, "LocalisationData_Demo.asset"));
#else
        data = AssetDatabase.LoadAssetAtPath<LocalisationData>(Path.Combine(folderPath, "LocalisationData.asset"));
#endif        

        keys.Clear();

        if(data == null)
        {
            return;
        }

        foreach (LocalisationDataValue dataValue in data.rawData)
        {
            keys.Add(dataValue.key);
        }
    }

    int FindCurIndex(string targetString)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i] == targetString)
            {
                return i;
            }
        }
        return 0;
    }
}
