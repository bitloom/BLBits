using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*[CustomEditor(typeof(LocalisationData))]
public class LocalisationDataInspector : Editor
{
    public override void OnInspectorGUI()
    {
        LocalisationData targetComponent = target as LocalisationData;

        EditorGUILayout.LabelField("Localisation Data");
        EditorGUILayout.LabelField("num keys: " + targetComponent.rawData.Count);
        foreach (LocalisationDataValue dataValue in targetComponent.rawData)
        {
            string printString = dataValue + ": ";

            for(int i = 0; i < dataValue.values.Length; i++)
            {
                printString += i.ToString() + " - ";
                printString += dataValue.values[i];
                if (i < dataValue.values.Length-1)
                {
                    printString += ", ";
                }
            }


            EditorGUILayout.LabelField(printString);
        }
    }
}*/
