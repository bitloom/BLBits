using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalisationDataValue
{
    public string key;
    public string[] values;
}

public class LocalisationData : ScriptableObject
{
    public int numLanguages = 1;
    public List<string> languageCodes = new List<string>();
    public List<LocalisationDataValue> rawData = new List<LocalisationDataValue>();
}


