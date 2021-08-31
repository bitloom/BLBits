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

    [MenuItem("Build/CSV Importer")]
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
                    if(curCharIndex + 1 < buffer.Length && buffer[curCharIndex + 1].ToString() == "\"")
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
            for(int i = 0; i < stringValues[key].Length; i++)
            {
                newDataPoint.values[i] = stringValues[key][i];
            }

            curData.rawData.Add(newDataPoint);
        }

        curData.numLanguages = numLanguages;

        AssetDatabase.CreateAsset(curData, path);        
        Selection.activeObject = curData;
        EditorGUIUtility.PingObject(curData);
        EditorUtility.SetDirty(curData);

        AssetDatabase.SaveAssets();

        // Update Chinese and Japanese text files      
        string chineseText = GetAllTextForLanguage(languageIndex: 5);
        chineseText = RemoveAlphanumericalCharacters(chineseText); // The alpha numerical characters are handled by falling back to the regular Phont
        File.WriteAllText("Assets/Art/Universal_Art/UI_Art/Fonts/ChineseCharacters.txt", chineseText);
        Debug.Log("ChineseCharacters.txt updated!");

        string japaneseText = GetAllTextForLanguage(languageIndex: 6);
        japaneseText = RemoveAlphanumericalCharacters(japaneseText);
        File.WriteAllText("Assets/Art/Universal_Art/UI_Art/Fonts/JapaneseCharacters.txt", japaneseText);
        Debug.Log("JapaneseCharacters.txt updated!");

        AssetDatabase.SaveAssets();

        // TODO: figure out how to do this with code
        Debug.Log("Please update 'NotoSansSC-Bold SDF' and 'MPLUSRounded1c-ExtraBold SDF' now");
        
        //NotoSansSC-Bold SDF is for Chinese
        //TMP_FontAsset chineseFont = AssetDatabase.LoadAssetAtPath("Assets/Art/Universal_Art/UI_Art/Fonts/NotoSansSC-Bold SDF.asset", typeof(TMP_FontAsset)) as TMP_FontAsset;
        //chineseFont.atlaste

        //StringBuilder builder = new StringBuilder();
        //foreach (string value in chineseData.values)
        //{
        //    builder.Append(value);
        //}

        //FontAssetCreationSettings fontAssetCreationSettings = new FontAssetCreationSettings
        //{

        //    padding = 5,
        //    //packingMode
        //    atlasWidth = 2048,
        //    atlasHeight = 2048,
        //    characterSequence = builder.ToString(),
        //    sourceFontFileName = "Assets/Art/Universal_Art/UI_Art/Fonts/ChineseCharacters.txt",
        //};

        //TMPro_FontAssetCreatorWindow.GetWindow<TMPro_FontAssetCreatorWindow>().atlas
        //TMPro_FontAssetCreatorWindow.ShowFontAtlasCreatorWindow(TMP_FontAsset fontAsset);

        //MPLUSRounded1c-ExtraBold SDF is for Japanese
        //TMP_FontAsset japaneseFont = AssetDatabase.LoadAssetAtPath("Assets/Art/Universal_Art/UI_Art/Fonts/MPLUSRounded1c-ExtraBold SDF.asset", typeof(TMP_FontAsset)) as TMP_FontAsset;
    }
    
    private static string RemoveAlphanumericalCharacters(string text)
    {        
        return Regex.Replace(input: text, pattern: "[a-zA-Z0-9]", replacement: "");
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
