using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalisedFontManager : MonoBehaviour
{
    public static TMP_FontAsset GetLanguageFont(int targetLanguage = -1)
    {
        if(instance != null)
        {
            if(targetLanguage < 0)
            {
                targetLanguage = LocalisationManager.GetCurLanguage();
            }

            return instance.GetFont(targetLanguage);
        }
        return null;
    }

    public static void CreateInstance(GameObject targetPrefab)
    {
        if(targetPrefab == null)
        {
            targetPrefab = Resources.Load<GameObject>("LocalisedFontManager");
        }
        
        if(targetPrefab != null)
        {
            instance = Instantiate(targetPrefab).GetComponent<LocalisedFontManager>();
        }
    }

    private static LocalisedFontManager instance;

    public TMP_FontAsset defaultFont;
    public TMP_FontAsset[] fonts;
    
    private void OnEnable()
    {
        if (LocalisedFontManager.instance != null && LocalisedFontManager.instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            LocalisedFontManager.instance = this;
        }
    }

    private void Start()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private TMP_FontAsset GetFont(int targetLanguage)
    {
        if(targetLanguage >= 0 && targetLanguage < fonts.Length && fonts[targetLanguage] != null)
        {
            return fonts[targetLanguage];
        }
        return defaultFont;
    }
}
