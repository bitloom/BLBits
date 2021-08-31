using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextLocaliser : MonoBehaviour
{
    [LocalisedText, SerializeField]
    private string key;
    
    [Header("Platform Specific Text")]
    public bool usePlatformSpecificOverrides;
    [LocalisedText, SerializeField] private string XB1Override;
    [LocalisedText, SerializeField] private string PS4Override;
    [LocalisedText, SerializeField] private string SwitchOverride;

    private Text uiText;
    private TextMesh gameText;
    private TMP_Text tmpText;

    public bool localationOverride = false;

    void OnEnable()
    {
        LocalisationManager.OnLanguageUpdate += UpdateText;
        UpdateText();        
    }

    void OnDisable()
    {
        LocalisationManager.OnLanguageUpdate -= UpdateText;
    }    

    public void Start () 
    {
        uiText = GetComponent<Text>();
        gameText = GetComponent<TextMesh>();
        tmpText = GetComponent<TMP_Text>();
        UpdateText();
    }
    
    void UpdateText()
    {
        string text = (localationOverride ? GetKey() : LocalisationManager.GetText(GetKey()));
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
            tmpText.font = LocalisedFontManager.GetLanguageFont();

            tmpText.SetText(text);

            tmpText.ForceMeshUpdate();
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
