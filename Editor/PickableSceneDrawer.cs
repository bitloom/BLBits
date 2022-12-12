using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

[CustomPropertyDrawer(typeof(PickableSceneAttribute))]
public class PickableSceneDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(property.propertyType == SerializedPropertyType.String)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            //SceneAsset previousScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/" + property.stringValue + ".unity");
            SceneAsset previousScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);

            EditorGUI.BeginChangeCheck();

            var newScene = EditorGUI.ObjectField(position, label, previousScene, typeof(SceneAsset), false) as SceneAsset;

            if(EditorGUI.EndChangeCheck())
            {
                string newPath = AssetDatabase.GetAssetPath(newScene);
                //newPath = Regex.Replace(newPath, "Assets/", "");
                //newPath = Regex.Replace(newPath, ".unity", "");
                property.stringValue = newPath;
            }

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(position, "Pickable Scene can only be applied to a string");
        }
    }
}
