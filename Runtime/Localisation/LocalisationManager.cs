using System.Collections.Generic;
using System.Text.RegularExpressions;
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

    

    public static int SystemCodeToIndex(string languageCode)
    {
        if(textData == null || languageCode == "en-US")
        {
            return 0;
        }

        int languageIndex = textData.languageCodes.IndexOf(languageCode.ToUpper());
        if (languageIndex >= 0)
        {
            return languageIndex;
        }
        return -1;
    }


    public static bool CurLanguageIs(string languageCode)
    {
        if(textData == null)
        {
            return false;
        }

        int index = textData.languageCodes.IndexOf(languageCode);
        if(index<0)
        {
            return false;
        }

        return index == curLanguage;
    }

    public static string GetLanguageCode(int languageIndex)
    {
        if(textData == null || languageIndex < 0 || languageIndex >= textData.languageCodes.Count)
        {
            return string.Empty;
        }

        return textData.languageCodes[languageIndex];
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

    public static void Initialise(LocalisationData data)
    {
        if (!initialised || textData == null)
        {
            textData = data;
            initialised = true;

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
            Debug.LogWarning("Trying to access Uninitialised Localisation Data!");
            return "ERROR: Localisation Failed To Load";
        }

        string returnString = "MISSING LANGUAGE NAME";
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
        if (textData == null || curLanguage < 0)
        {
            Debug.LogWarning("Trying to access Uninitialised Localisation Data!");
            return "ERROR: Localisation Failed To Load";
        }

        if (text != null && text.ContainsKey(key))
        {
            if (text[key].Length > curLanguage)
            {
                if (text[key][curLanguage] == "")
                {
                    return text[key][0];
                }
                else
                {
                    return text[key][curLanguage];
                }
            }
        }
        return string.Format("MISSING LOCALISED TEXT (Key: {0})", key);
    }

    public static string GetTextForLanguage(string key, int language)
    {
        if (textData == null)
        {
            Debug.LogWarning("Trying to access Uninitialised Localisation Data!");
            return "ERROR";
        }

        if (text != null && text.ContainsKey(key))
        {
            if (text[key].Length > language)
            {
                if (text[key][language] == "")
                {
                    return text[key][0];
                    //return string.Format("MISSING TEXT (Key: {0} Language: {1})", key, GetLanguageName(curLanguage));
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
            Debug.LogWarning("Trying to access Uninitialised Localisation Data!");
            return "ERROR: Failed to load Localisation";
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

    public static LocalisationData GetData()
    {
        return textData;
    }

    public static void ApplyText(TMPro.TMP_Text targetText, string targetKey, bool skipLocalisation = false, bool skipUpdateLineSpacing = false)
    {
        if(string.IsNullOrEmpty(targetKey) || targetText == null)
        {
            return;
        }

        string targetString = skipLocalisation ? targetKey : GetText(targetKey);

        bool isArabic = CurLanguageIs("AR");
        if(isArabic)
        {
            targetString = ArabicSupport.ArabicFixer.Fix(targetString, true);
        }

        var targetFont = LocalisedFontManager.GetLanguageFont();
        if (targetFont != null)
        {
            targetText.font = targetFont;
        }

        targetText.text = targetString;

        if (skipUpdateLineSpacing == false)
        {
            UpdateLineSpacing(targetText, targetFont, isArabic);
        }
    }

    public static void UpdateLineSpacing(TMPro.TMP_Text targetText, TMPro.TMP_FontAsset targetFont, bool isArabic)
    {
        if(targetFont == null || targetText == null)
        {
            return;
        }

        if (isArabic)
        {
            if (targetText.enableAutoSizing)
            {
                targetText.ForceMeshUpdate();
                float curFontSize = targetText.fontSize;
                targetText.enableAutoSizing = false;
                targetText.fontSize = curFontSize;
            }

            targetText.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;

            if (targetFont != null)
            {
                targetText.lineSpacing = (targetFont.faceInfo.lineHeight + targetFont.faceInfo.ascentLine) * -2;
            }
        }
        else if (targetText.lineSpacing < 0)
        {
            targetText.lineSpacing = 0;
            targetText.enableAutoSizing = true;
        }
    }

    public static string GetDateString(int dayOfMonth, int dayOfWeek, int month)
    {
        if(textData == null || curLanguage < 0)
        {
            return string.Empty;
        }

        string dayString = GetText("DAY_" + (dayOfWeek).ToString("00"));
        string monthString = GetText("MONTH_" + (month).ToString("00"));

        string dateFormat = GetText("DATE_FORMAT");

        string returnString = Regex.Replace(dateFormat, "{dayname}", dayString);
        returnString = Regex.Replace(returnString, "{month}", monthString);
        returnString = Regex.Replace(returnString, "{day}", dayOfMonth.ToString());

        return returnString;
    }
}
