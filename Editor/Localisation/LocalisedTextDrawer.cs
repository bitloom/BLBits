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

    private SearchablePopup searchablePopup;
    private int curKeyIndex = 0;
    private bool indexDirty = false;

    void OnEnable()
    {
        SetupKeys();
        //curKeyIndex = FindCurIndex();
    }

    void OnPopupClose(int selectedItem)
    {
        if (selectedItem >= 0)
        {
            curKeyIndex = selectedItem;
            if (selectedItem >= 0)
            {
                indexDirty = true;
            }
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (keys.Count == 0)
        {
            SetupKeys();
        }

        //int curKeyIndex = FindCurIndex(property.stringValue);

        Rect stringRect = position;
        stringRect.width -= 25;

        Rect buttonRect = position;
        buttonRect.x += position.width - 24;
        buttonRect.width = 24;

        Rect popupRect = position;
        popupRect.x += EditorGUIUtility.labelWidth;
        popupRect.width -= EditorGUIUtility.labelWidth;

        EditorGUI.PropertyField(stringRect, property, label);

        if (GUI.Button(buttonRect, "<"))
        {
            PopupWindow.Show(position, searchablePopup);
        }

        if (indexDirty)
        {
            indexDirty = false;
            property.stringValue = keys[curKeyIndex];
        }

        /*EditorGUI.BeginChangeCheck();

        curKeyIndex = EditorGUI.Popup(position, "Text Key", curKeyIndex, keys.ToArray());

        if (EditorGUI.EndChangeCheck() && keys.Count > 0)
        {
            property.stringValue = keys[curKeyIndex];
        }*/

        EditorGUI.EndProperty();
    }

    void SetupKeys()
    {
        string folderPath = "Assets/Resources/GameData";
        data = AssetDatabase.LoadAssetAtPath<LocalisationData>(Path.Combine(folderPath, "LocalisationData.asset"));

        keys.Clear();

        if (data == null)
        {
            return;
        }

        foreach (LocalisationDataValue dataValue in data.rawData)
        {
            keys.Add(dataValue.key);
        }

        searchablePopup = new SearchablePopup(keys.ToArray());
        searchablePopup.onWindowClosed += OnPopupClose;
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
