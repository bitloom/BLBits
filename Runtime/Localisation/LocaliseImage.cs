using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LocaliseImage : MonoBehaviour
{
    public Sprite[] languageSprites;
    private Image targetImage;

    private void OnEnable()
    {
        LocalisationManager.OnLanguageUpdate += UpdateImage;
        UpdateImage();
    }

    private void OnDisable()
    {
        LocalisationManager.OnLanguageUpdate -= UpdateImage;
    }

    private void Awake()
    {
        UpdateImage();
    }

    private void UpdateImage()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }
        if(targetImage == null)
        {
            return;
        }

        int targetLanguage = LocalisationManager.GetCurLanguage();
        if (targetLanguage != -1)
        {
            if (targetLanguage >= languageSprites.Length || languageSprites[targetLanguage] == null)
            {
                Debug.LogError("Missing Localised Sprite for Language: " + LocalisationManager.GetLanguageName(targetLanguage), this);
            }
            else
            {
                targetImage.sprite = languageSprites[targetLanguage];
            }
        }
    }
}
