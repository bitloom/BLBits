using UnityEditor;
using UnityEngine;

public class CSVImporterWindow : EditorWindow
{
    private string filePath = "";
    private string assetName = "LocalisationData";
    private int numLanguages = 7;

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

        numLanguages = EditorGUILayout.IntField("Number of Languages", numLanguages);

        GUILayout.Space(10);

        assetName = EditorGUILayout.TextField("Asset Name", assetName);

        GUILayout.Space(10);

        if (filePath.Length != 0)
        {
            if (GUILayout.Button("Import File"))
            {
                CSVImporter.LoadCSV(filePath, assetName, numLanguages);
                Close();
            }
        }
    }
}
