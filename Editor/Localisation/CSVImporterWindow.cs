using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CSVImporterWindow : EditorWindow
{
    private static string filePath = "";
    private static string assetName = "LocalisationData";
    private static List<string> languages = new List<string>( new string[] { "en" });
    private static string languagesString = "EN";
    private static string languagesSeparator = "\t";

    void OnGUI()
    {
        GUILayout.Space(10);

        GUILayout.Label("The Magic CSV Importer!");

        GUILayout.Space(10);

        filePath = EditorGUILayout.TextField("CSV File", filePath);
        if (GUILayout.Button("Choose File"))
        {
            filePath = EditorUtility.OpenFilePanel("Choose a CSV Please", "", "csv");
        }

        GUILayout.Space(10);

        EditorGUI.BeginChangeCheck();

        languagesSeparator = EditorGUILayout.TextField("Separator", languagesSeparator);
        languagesString = EditorGUILayout.TextField("Language Codes", languagesString);

        if (EditorGUI.EndChangeCheck())
        {
            string[] codes = languagesString.Split(languagesSeparator);
            languages.Clear();
            languages.AddRange(codes);
        }

        int curIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;

        for(int i = 0; i < languages.Count; i++)
        {
            EditorGUILayout.LabelField(languages[i]);
        }

        EditorGUI.indentLevel = curIndentLevel;

        GUILayout.Space(10);

        assetName = EditorGUILayout.TextField("Asset Name", assetName);

        GUILayout.Space(10);

        if (filePath.Length != 0)
        {
            if (GUILayout.Button("Import File"))
            {
                CSVImporter.LoadCSV(filePath, assetName, languages);
                Close();
            }
        }
    }
}
