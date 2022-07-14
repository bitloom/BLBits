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

    public static void LoadCSV(string path, string assetName = "", int numLanguages = 1)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Provided file doesn't exist");
            return;
        }

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
        WriteAsset(assetName, numLanguages);
    }

    private static void WriteAsset(string assetName, int numLanguages)
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

        curData.numLanguages = numLanguages;

        AssetDatabase.CreateAsset(curData, path);
        Selection.activeObject = curData;
        EditorGUIUtility.PingObject(curData);
        EditorUtility.SetDirty(curData);

        AssetDatabase.SaveAssets();

        if (numLanguages > 5)
        {
            // Update Chinese and Japanese text files      
            string chineseText = GetAllTextForLanguage(languageIndex: 5);
            chineseText = RemoveAlphanumericalCharacters(chineseText); // The alpha numerical characters are handled by falling back to the regular Phont
            File.WriteAllText("Assets/Art/Universal_Art/UI_Art/Fonts/ChineseCharacters.txt", chineseText);
            Debug.Log("ChineseCharacters.txt updated!");
        }

        if (numLanguages > 6)
        {
            string japaneseText = GetAllTextForLanguage(languageIndex: 6);
            japaneseText = RemoveAlphanumericalCharacters(japaneseText);
            File.WriteAllText("Assets/Art/Universal_Art/UI_Art/Fonts/JapaneseCharacters.txt", japaneseText);
            Debug.Log("JapaneseCharacters.txt updated!");
        }

        AssetDatabase.SaveAssets();

        Debug.Log("Please update 'NotoSansSC-Bold SDF' and 'MPLUSRounded1c-ExtraBold SDF' now");
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
