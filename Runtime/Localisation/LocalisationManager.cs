using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalisationManager
{
    public static int NUM_LANGUAGES;

    private static bool initialised = false;
    private static LocalisationData textData = null;
    private static Dictionary<string, string[]> text;

    private static int curLanguage = -1;

    public delegate void LanguageUpdated();
    public static event LanguageUpdated OnLanguageUpdate;

    public static string LocalisationFileName = string.Empty;

    public static int SystemCodeToIndex(string language)
    {
        switch (language)
        {
            case ("en-US"):
                return (0);
            default:
                break;
        }

        int offset = language.IndexOf('-');
        if (0 < offset)
        {
            language = language.Substring(0, offset);
        }

        switch(language)
        {
            case ("fr"):    //French
                return (1);
            case ("it"):    //Italy
                return (2);
            case ("de"):    //german
                return (3);
            case ("es"):    //spanish
                return (4);
            case ("zh"):    //Chinese
                return (5);
            case ("ja"):    //Japanese
                return (6);
            case ("en"):
            default:
                return (0);
        }
    }

    public static void Initialise(string localisationDataName)
    {
        if (!initialised || textData == null)
        {
            initialised = true;
            textData = Resources.Load(localisationDataName) as LocalisationData;

            Debug.Log("Initialising Localisation Manager...");

            if (textData)
            {
                Debug.Log("Successfully Loaded Text Data");

                CreateDictionary();
            }
        }
    }

    public static string GetLanguageName(int language)
    {
        if (textData == null)
        {
            Debug.LogError("Trying to access Uninitialised Localisation Data!");
            return "ERROR";
        }

        string returnString = "MISSING LANGUAGE";
        if (text.Count > 0)
        {
            string[] languages;
            if(text.TryGetValue("LANGUAGE", out languages))
            {
                if (languages[language] != "")
                {
                    return languages[language];
                }
            }
        }
        return returnString;
    }

    public static string GetText(string key)
    {
        if (textData == null)
        {
            Debug.LogError("Trying to access Uninitialised Localisation Data!");
            return "ERROR";
        }

        if (text != null && text.ContainsKey(key))
        {
            if (text[key].Length > curLanguage)
            {
                if (text[key][curLanguage] == "")
                {
                    return  string.Format("MISSING TEXT (Key: {0} Language: {1})", key, GetLanguageName(curLanguage));
                }
                return text[key][curLanguage];
            }
        }
        return string.Format("MISSING LOCALISED TEXT (Key: {0})", key);
    }

    public static string GetTextForLanguage(string key, int language)
    {
        if (textData == null)
        {
            Debug.LogError("Trying to access Uninitialised Localisation Data!");
            return "ERROR";
        }

        if (text != null && text.ContainsKey(key))
        {
            if (text[key].Length > language)
            {
                if (text[key][language] == "")
                {
                    return string.Format("MISSING TEXT (Key: {0} Language: {1})", key, GetLanguageName(curLanguage));
                }
                return text[key][language];
            }
        }
        return string.Format("MISSING LOCALISED TEXT (Key: {0})", key);

    }

    public static string GetText(string key, params object[] args)
    {
        if (textData == null)
        {
            Debug.LogError("Trying to access Uninitialised Localisation Data!");
            return "ERROR";
        }

        string result = GetText(key);
        try
        {
            result = string.Format(result, args);
            return (result);
        }
        catch
        {
            return (result);
        }
    }

    public static int GetCurLanguage()
    {
        return curLanguage;
    }

    public static void SetLanguage(int newLanguage)
    {
        curLanguage = newLanguage;
        if (OnLanguageUpdate != null)
        {
            OnLanguageUpdate();
        }
    }

    public static void IncrementLanguage()
    {
        int targetLanguage = curLanguage + 1;
        if(targetLanguage >= NUM_LANGUAGES)
        {
            targetLanguage = 0;
        }

        SetLanguage(targetLanguage);
    }

    public static void DecremenetLanguage()
    {
        int targetLanguage = curLanguage - 1;
        if (targetLanguage < 0)
        {
            targetLanguage = NUM_LANGUAGES - 1;
        }

        SetLanguage(targetLanguage);
    }

    private static void CreateDictionary()
    {
        NUM_LANGUAGES = textData.numLanguages;
        text = new Dictionary<string, string[]>();
        foreach (LocalisationDataValue dataValue in textData.rawData)
        {
            text[dataValue.key] = dataValue.values;
        }
    }
}
