using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArabicSupport;

public class TextLocaliser : MonoBehaviour
{
    [LocalisedText, SerializeField]
    private string key;

    [Tooltip("Select this if you want to use the raw input")]
    public bool overrideText = false;

    [Header("Platform Specific Text")]
    public bool usePlatformSpecificOverrides;
    [LocalisedText, SerializeField] private string XB1Override;
    [LocalisedText, SerializeField] private string PS4Override;
    [LocalisedText, SerializeField] private string SwitchOverride;

    private Text uiText;
    private TextMesh gameText;
    private TMP_Text tmpText;

    public bool IsArabic { get; set; } = false;

    void OnEnable()
    {
        LocalisationManager.OnLanguageUpdate += UpdateText;
        UpdateText();
    }

    void OnDisable()
    {
        LocalisationManager.OnLanguageUpdate -= UpdateText;
    }

    public void Start()
    {
        uiText = GetComponent<Text>();
        gameText = GetComponent<TextMesh>();
        tmpText = GetComponent<TMP_Text>();
        UpdateText();
    }

    void UpdateText()
    {
        string text = (overrideText ? GetKey() : LocalisationManager.GetText(GetKey()));
        if (uiText)
        {
            uiText.text = text;
        }

        if (gameText)
        {
            gameText.text = text;
        }

        if (tmpText)
        {
            var targetFont = LocalisedFontManager.GetLanguageFont();
            if(targetFont != null)
            {
                tmpText.font = targetFont;
            }

            bool isArabic = false;
            if(IsArabic)
            {
                isArabic = true;
            }
            else if(overrideText == false && LocalisationManager.CurLanguageIs("AR"))
            {
                isArabic = true;
            }

            if(isArabic)
            {
                text = ArabicFixer.Fix(text, true);
            }
            else // make sure there's no unecessary rtl going on
            {
                tmpText.isRightToLeftText = false;
            }

            tmpText.SetText(text);

            tmpText.ForceMeshUpdate();

            LocalisationManager.UpdateLineSpacing(tmpText, targetFont, isArabic);
        }
    }

    public string GetKey()
    {
        if (!usePlatformSpecificOverrides)
        {
            return key;
        }
        else
        {
#if UNITY_XBOXONE
            return XB1Override;
#elif UNITY_PS4
            return PS4Override;            
#elif UNITY_SWITCH
            return SwitchOverride;
#elif UNITY_STANDALONE
            return key;
#else
            return null;
#endif
        }
    }

    public void SetKey(string newKey)
    {
        if (!usePlatformSpecificOverrides)
        {
            key = newKey;
        }
        else
        {
#if UNITY_XBOXONE
            XB1Override = newKey;
#elif UNITY_PS4
            PS4Override = newKey;
#elif UNITY_SWITCH
            SwitchOverride = newKey;
#elif UNITY_STANDALONE
            key = newKey;
#else
            Debug.LogError("Unknown platform!");
            key = newKey;
#endif
        }

        UpdateText();
    }
}
