using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Text.RegularExpressions;

public class CSVImporter
{
    private static Dictionary<string, string[]> stringValues;

    [MenuItem("Tools/Bit Loom/CSV Importer")]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow(typeof(CSVImporterWindow));
    }

    public static void LoadCSV(string path, string assetName, List<string> languageCodes)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Provided file doesn't exist");
            return;
        }

        int numLanguages = languageCodes.Count;

        stringValues = new Dictionary<string, string[]>();

        string buffer = File.ReadAllText(path);

        bool inQuote = false;
        int curCharIndex = 0;
        string curKey = "";
        string curWord = "";
        int curLanguage = -1;

        while (curCharIndex < buffer.Length)
        {
            string curChar = buffer[curCharIndex].ToString();

            //"HE""ll""o",","

            if (inQuote)
            {
                if (curChar == "\"")
                {
                    if (curCharIndex + 1 < buffer.Length && buffer[curCharIndex + 1].ToString() == "\"")
                    {
                        curCharIndex++;
                    }
                    else
                    {
                        inQuote = false;
                        curChar = "";
                    }
                }
            }
            else
            {
                if (curChar == "\"")
                {
                    inQuote = true;
                    curChar = "";
                }
                else if (curChar == ",")
                {
                    if (curLanguage < 0)
                    {
                        curKey = curWord;
                        stringValues[curKey] = new string[numLanguages];
                    }
                    else if (curLanguage < numLanguages) //fill in a language for the key
                    {
                        curWord = curWord;
                        stringValues[curKey][curLanguage] = curWord;
                    }

                    curLanguage++;
                    curWord = "";
                    curCharIndex++;
                    continue;
                }
                else if (curChar == "\n")
                {
                    if (curLanguage < 0)
                    {
                        curWord = curWord;
                        curKey = curWord;
                        stringValues[curKey] = new string[numLanguages];
                    }
                    else if (curLanguage < numLanguages) //fill in a language for the key
                    {
                        stringValues[curKey][curLanguage] = curWord;
                    }

                    curLanguage = -1;
                    curCharIndex++;
                    curWord = "";
                    curKey = "";
                    continue;
                }
            }


            curWord += curChar;

            curCharIndex++;
        }

        if (curLanguage < 0)
        {
            curKey = curWord;
            stringValues[curKey] = new string[numLanguages];
        }
        else if (curLanguage < numLanguages) //fill in a language for the key
        {
            stringValues[curKey][curLanguage] = curWord;
        }

        if (assetName == "")
        {
            assetName = "LocalisationData";
        }
        WriteAsset(assetName, languageCodes);
    }

    private static void WriteAsset(string assetName, List<string> languageCodes)
    {
        Debug.Log("Writing Localisation Asset");

        string path = "Assets/Resources/GameData/" + assetName + ".asset";

        LocalisationData curData = ScriptableObject.CreateInstance<LocalisationData>();
        curData.rawData = new List<LocalisationDataValue>();

        foreach (string key in stringValues.Keys)
        {
            //string printString = key + ": ";
            LocalisationDataValue newDataPoint = new LocalisationDataValue();
            newDataPoint.key = key;
            newDataPoint.values = new string[stringValues[key].Length];
            for (int i = 0; i < stringValues[key].Length; i++)
            {
                newDataPoint.values[i] = RemoveReturnCharacters(stringValues[key][i]);
            }

            curData.rawData.Add(newDataPoint);
        }

        curData.numLanguages = languageCodes.Count;
        curData.languageCodes = new List<string>();
        curData.languageCodes.AddRange(languageCodes);

        AssetDatabase.CreateAsset(curData, path);
        Selection.activeObject = curData;
        EditorGUIUtility.PingObject(curData);
        EditorUtility.SetDirty(curData);

        AssetDatabase.SaveAssets();

        for(int i = 0; i < languageCodes.Count; i++)
        {
            string checkCode = languageCodes[i].ToLower();
            switch(checkCode)
            {
                case "th":
                case "kr":
                case "scn":
                case "ru":
                case "tch":
                case "sch":
                case "ar":
                case "jp":
                    WriteCharacterFile(checkCode, i);
                    break;
            }
        }

        AssetDatabase.SaveAssets();
    }

    private static void WriteCharacterFile(string languageCode, int languageIndex)
    {
        string languageText = GetAllTextForLanguage(languageIndex);
        languageText = RemoveAlphanumericalCharacters(languageText); // The alpha numerical characters are handled by falling back to the regular font
        string path = string.Format("Assets/Misc/LanguageCharacters_{0}.txt", languageCode);
        TextAsset newTextAsset = new TextAsset(languageText);
        AssetDatabase.CreateAsset(newTextAsset, path);

        File.WriteAllText(path, languageText);
        
        AssetDatabase.SaveAssets();


        Debug.LogFormat("Character file for {0} updated!", languageCode);
    }

    private static string RemoveAlphanumericalCharacters(string text)
    {
        return Regex.Replace(input: text, pattern: "[a-zA-Z0-9]", replacement: "");
    }

    private static string RemoveReturnCharacters(string text)
    {
        return Regex.Replace(input: text, pattern: "\r", replacement: "");
    }

    private static string GetAllTextForLanguage(int languageIndex)
    {
        string languageText = "";
        foreach (string key in stringValues.Keys)
        {
            languageText += stringValues[key][languageIndex] + "\n";
        }

        return languageText;
    }
}
