using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalisedFontManager : MonoBehaviour
{
    public static TMP_FontAsset GetLanguageFont()
    {
        if(instance != null)
        {
            return instance.GetFont(LocalisationManager.GetCurLanguage());
        }
        return null;
    }

    private static LocalisedFontManager instance;

    public TMP_FontAsset defaultFont;
    public TMP_FontAsset[] fonts;
    
    private void OnEnable()
    {
        LocalisedFontManager.instance = this;
    }

    private TMP_FontAsset GetFont(int targetLanguage)
    {
        if(targetLanguage < fonts.Length && fonts[targetLanguage] != null)
        {
            return fonts[targetLanguage];
        }
        return defaultFont;
    }
}
