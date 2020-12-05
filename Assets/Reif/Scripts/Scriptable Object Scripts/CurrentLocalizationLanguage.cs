using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Reif Custom Data/Current Localization Language")]
public class CurrentLocalizationLanguage : ScriptableObject 
{
    public event Action OnLanguageChanged;

    public Language language { get; private set; }

    private Dictionary<string, string> localizationPairs;

    /// <summary>
    /// Stores newly loaded localized data
    /// </summary>
    /// <param name="localizationData">Localization data loaded from a localization file</param>
    public void ChangeLocalizationLanguage(LocalizationData localizationData)
    {
        language = localizationData.language;
        localizationPairs = new Dictionary<string, string>();

        for (int i = 0; i < localizationData.items.Count; ++i)
            localizationPairs.Add(localizationData.items[i].key, localizationData.items[i].value);

        OnLanguageChanged?.Invoke();
    }

    /// <summary>
    /// Returns localized value for a key
    /// </summary>
    public string GetLocalizedValue(string key)
    {
        if (localizationPairs.ContainsKey(key))
            return localizationPairs[key];

        return Constants.MISSING_TEXT_STRING;
    }

    /// <summary>
    /// Returns boolean whether locazation language is loaded
    /// </summary>
    public bool IsLoaded()
    {
        return (localizationPairs != null && localizationPairs.Count > 0);
    }
}
